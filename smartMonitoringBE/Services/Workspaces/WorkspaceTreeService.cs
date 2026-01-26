using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO.Workspace;
using smartMonitoringBE.Security;

namespace smartMonitoringBE.Services.Workspaces;

public sealed class WorkspaceTreeService : IWorkspaceTreeService
{
    private readonly SmartMonitoringDbContext _db;
    private readonly IAuthzContextAccessor _authz;

    public WorkspaceTreeService(SmartMonitoringDbContext db, IAuthzContextAccessor authz)
    {
        _db = db;
        _authz = authz;
    }

    public async Task<WorkspaceTreeResponseDto> GetWorkspaceTreeAsync(Guid accountId, CancellationToken ct)
    {
        var authzCtx = await _authz.GetAsync(accountId, ct);

        // 1) which workspaces can user see?
        var workspaceIds = await ScopeChecks.GetAllowedWorkspaceIds(_db, authzCtx, accountId, ct);
        if (workspaceIds.Count == 0)
            return new WorkspaceTreeResponseDto { AccountId = accountId };

        // 2) load workspaces
        var workspaces = await _db.Workspaces
            .AsNoTracking()
            .Where(w => w.AccountId == accountId && workspaceIds.Contains(w.Id) && w.IsActive)
            .Select(w => new { w.Id, w.Name, w.Code, w.Type })
            .ToListAsync(ct);

        // 3) load nodes (flat)
        var nodes = await _db.WorkspaceNodes
            .AsNoTracking()
            .Where(n => workspaceIds.Contains(n.WorkspaceId) && n.IsActive)
            .Select(n => new
            {
                n.Id, n.WorkspaceId, n.ParentId, n.Name, n.Code, n.Type, n.IconType, n.SortOrder
            })
            .ToListAsync(ct);

// Convert to List<dynamic>
        var nodesDyn = nodes.Cast<dynamic>().ToList();

// 4) scope filtering
        var filtered = ScopeChecks.FilterNodesByScope(nodesDyn, authzCtx);

// 5) build trees
        var response = new WorkspaceTreeResponseDto { AccountId = accountId };

        foreach (var w in workspaces.OrderBy(x => x.Name))
        {
            var wsNodes = filtered
                .Where(n => (Guid)n.WorkspaceId == w.Id)
                .ToList();

            response.Workspaces.Add(new WorkspaceTreeDto
            {
                Id = w.Id,
                Name = w.Name,
                Code = w.Code,
                Type = w.Type,
                Nodes = BuildTree(wsNodes) // BuildTree must accept List<dynamic>
            });
        }

        return response;
    }

    private static List<WorkspaceNodeTreeDto> BuildTree(List<dynamic> flat)
    {
        var map = flat.ToDictionary(
            x => (Guid)x.Id,
            x => new WorkspaceNodeTreeDto
            {
                Id = x.Id,
                ParentId = x.ParentId,
                Name = x.Name,
                Code = x.Code,
                Type = x.Type,
                IconType = x.IconType,
                SortOrder = x.SortOrder
            });

        var roots = new List<WorkspaceNodeTreeDto>();

        foreach (var n in map.Values)
        {
            if (n.ParentId is Guid pid && map.TryGetValue(pid, out var parent))
                parent.Children.Add(n);
            else
                roots.Add(n);
        }

        void Sort(List<WorkspaceNodeTreeDto> list)
        {
            list.Sort((a,b) =>
            {
                var c = a.SortOrder.CompareTo(b.SortOrder);
                return c != 0 ? c : string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
            });
            foreach (var item in list) Sort(item.Children);
        }

        Sort(roots);
        return roots;
    }
}