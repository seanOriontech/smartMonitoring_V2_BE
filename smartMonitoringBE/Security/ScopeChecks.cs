using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities.Structure;
using smartMonitoringBE.Infrastructure.Data;

namespace smartMonitoringBE.Security;


public sealed record WorkspaceNodeRow(
    Guid Id,
    Guid WorkspaceId,
    Guid? ParentId,
    string Name,
    string? Code,
    WorkspaceNodeType Type,
    NodeIconType IconType,
    int SortOrder
);

public static class ScopeChecks
{
    public static async Task<bool> HasWorkspaceAccess(SmartMonitoringDbContext db, AuthzContext ctx, Guid workspaceId, CancellationToken ct)
    {
        if (ctx.Scopes.HasAllWorkspaces) return true;
        if (ctx.Scopes.WorkspaceIds.Contains(workspaceId)) return true;

        if (ctx.Scopes.NodeIds.Count == 0) return false;

        return await db.Workspaces
            .AsNoTracking()
            .AnyAsync(n => n.Id == workspaceId && ctx.Scopes.NodeIds.Contains(n.Id), ct);
    }
    
      /// <summary>
    /// Returns workspaces user can see.
    /// If HasAll -> all workspaces for account.
    /// Else -> explicit workspace scopes + workspaces containing scoped nodes.
    /// </summary>
    public static async Task<HashSet<Guid>> GetAllowedWorkspaceIds(
        SmartMonitoringDbContext db,
        AuthzContext ctx,
        Guid accountId,
        CancellationToken ct)
    {
        if (ctx.Scopes.HasAllWorkspaces)
        {
            return  db.Workspaces
                .AsNoTracking()
                .Where(w => w.AccountId == accountId && w.IsActive)
                .Select(w => w.Id)
                .ToHashSet();
        }

        var allowed = new HashSet<Guid>(ctx.Scopes.WorkspaceIds);

        if (ctx.Scopes.NodeIds.Count > 0)
        {
            var fromNodes = await db.WorkspaceNodes
                .AsNoTracking()
                .Where(n => ctx.Scopes.NodeIds.Contains(n.Id))
                .Select(n => n.WorkspaceId)
                .Distinct()
                .ToListAsync(ct);

            foreach (var wid in fromNodes) allowed.Add(wid);
        }

        return allowed;
    }

    /// <summary>
    /// Expands a set of starting nodeIds to include ALL descendants (tree expansion).
    /// Uses iterative BFS (no recursion).
    /// </summary>
    public static async Task<HashSet<Guid>> ExpandToDescendants(
        SmartMonitoringDbContext db,
        HashSet<Guid> startNodeIds,
        CancellationToken ct)
    {
        var allowed = new HashSet<Guid>(startNodeIds);
        if (allowed.Count == 0) return allowed;

        // Load only what we need: Id + ParentId for nodes in the workspaces involved
        // But we don't know workspaces here; simplest: iterate by ParentId lookups.
        // For performance on big trees: restrict by workspace(s) before calling.
        var frontier = new Queue<Guid>(allowed);

        while (frontier.Count > 0)
        {
            var parentId = frontier.Dequeue();

            var children = await db.WorkspaceNodes
                .AsNoTracking()
                .Where(n => n.ParentId == parentId)
                .Select(n => n.Id)
                .ToListAsync(ct);

            foreach (var childId in children)
            {
                if (allowed.Add(childId))
                    frontier.Enqueue(childId);
            }
        }

        return allowed;
    }

    /// <summary>
    /// Filters a flat list of nodes to what the user may see.
    /// Keeps ancestors of allowed nodes so the FE can still render a valid tree.
    /// </summary>
    public static async Task<List<WorkspaceNode>> FilterNodesToScope(
        SmartMonitoringDbContext db,
        AuthzContext ctx,
        Guid workspaceId,
        List<WorkspaceNode> allNodesInWorkspace,
        CancellationToken ct)
    {
        if (ctx.Scopes.HasAllWorkspaces) return allNodesInWorkspace;

        // If user has whole-workspace access, no node pruning needed.
        if (ctx.Scopes.WorkspaceIds.Contains(workspaceId))
            return allNodesInWorkspace;

        // Node-scoped: allow those nodes + descendants.
        var scopedNodeIds = new HashSet<Guid>(
            ctx.Scopes.NodeIds // these are Guid in your loader (WorkspaceNodeId)
        );

        if (scopedNodeIds.Count == 0)
            return new List<WorkspaceNode>(); // no access to this workspace

        // Expand to descendants (within this workspace)
        // Optimisation: do expansion in-memory using allNodesInWorkspace (fast).
        var allowed = ExpandToDescendantsInMemory(allNodesInWorkspace, scopedNodeIds);

        // Keep ancestors too, otherwise you can’t navigate down the tree.
        AddAncestorsInMemory(allNodesInWorkspace, allowed);

        return allNodesInWorkspace.Where(n => allowed.Contains(n.Id)).ToList();
    }

    private static HashSet<Guid> ExpandToDescendantsInMemory(List<WorkspaceNode> nodes, HashSet<Guid> start)
    {
        var childrenByParent = nodes
            .Where(n => n.ParentId != null)
            .GroupBy(n => n.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());

        var allowed = new HashSet<Guid>(start);
        var q = new Queue<Guid>(start);

        while (q.Count > 0)
        {
            var parent = q.Dequeue();
            if (!childrenByParent.TryGetValue(parent, out var kids)) continue;

            foreach (var kid in kids)
            {
                if (allowed.Add(kid))
                    q.Enqueue(kid);
            }
        }

        return allowed;
    }

    private static void AddAncestorsInMemory(List<WorkspaceNode> nodes, HashSet<Guid> allowed)
    {
        var byId = nodes.ToDictionary(n => n.Id, n => n);

        var stack = new Stack<Guid>(allowed.ToList());
        while (stack.Count > 0)
        {
            var id = stack.Pop();
            if (!byId.TryGetValue(id, out var node)) continue;

            if (node.ParentId is Guid parentId && allowed.Add(parentId))
                stack.Push(parentId);
        }
    }
    
    public static List<dynamic> FilterNodesByScope(
        List<dynamic> nodes,
        AuthzContext authzCtx)
    {
        if (authzCtx.Scopes.HasAllWorkspaces)
            return nodes;

        var allowedWorkspaces = authzCtx.Scopes.WorkspaceIds;
        var allowedNodes = authzCtx.Scopes.NodeIds;

        // Fast lookup by Id
        var byId = nodes.ToDictionary(
            n => (Guid)n.Id,
            n => n
        );

        var keepIds = new HashSet<Guid>();

        // 1) Workspace-level access → include all nodes in workspace
        if (allowedWorkspaces.Count > 0)
        {
            foreach (var n in nodes)
            {
                var workspaceId = (Guid)n.WorkspaceId;

                if (allowedWorkspaces.Contains(workspaceId))
                    keepIds.Add((Guid)n.Id);
            }
        }

        // 2) Node-level access → include node + all ancestors
        if (allowedNodes.Count > 0)
        {
            foreach (var nodeId in allowedNodes)
            {
                if (!byId.TryGetValue(nodeId, out var current))
                    continue;

                while (true)
                {
                    var id = (Guid)current.Id;
                    if (!keepIds.Add(id))
                        break;

                    Guid? parentId = current.ParentId;

                    if (!parentId.HasValue)
                        break;

                    if (!byId.TryGetValue(parentId.Value, out current))
                        break;
                }
            }
        }

        return nodes
            .Where(n => keepIds.Contains((Guid)n.Id))
            .ToList();
    }
}
