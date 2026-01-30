using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities.Structure;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO.Workspace;

namespace smartMonitoringBE.Services.Workspaces;

public sealed class WorkspaceService : IWorkspaceService
{
    private readonly SmartMonitoringDbContext _db;

    public WorkspaceService(SmartMonitoringDbContext db)
    {
        _db = db;
    }

    // -------------------------
    // Workspaces
    // -------------------------

    public async Task<Workspace> CreateWorkspaceAsync(Guid accountId, WorkspaceCreateDto dto, CancellationToken ct = default)
    {
        var code = await EnsureUniqueWorkspaceCodeAsync(accountId, dto.Code ?? Slug(dto.Name), ct);

        var ws = new Workspace
        {
            Id = Guid.NewGuid(),
            AccountId =accountId,
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            Type = dto.Type,
            Code = code,
            PrimaryWorkspace = dto.PrimaryWorkspace,
            TimeZone = dto.TimeZone,
            //DefaultSiteId = d,
            IsActive = true,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        _db.Workspaces.Add(ws);
        await _db.SaveChangesAsync(ct);

        return ws;
    }

    public async Task<Workspace> UpdateWorkspaceAsync(Guid accountId,Guid workspaceId, WorkspaceUpdateDto dto, CancellationToken ct = default)
    {
        var ws = await _db.Workspaces.FirstOrDefaultAsync(x => x.Id == workspaceId, ct)
                 ?? throw new InvalidOperationException("Workspace not found.");

        ws.Name = dto.Name.Trim();
        ws.Description = dto.Description?.Trim();
        ws.IsActive = true;
        ws.PrimaryWorkspace = dto.PrimaryWorkspace;
        ws.TimeZone = dto.TimeZone;
        ws.Type = dto.workspaceType;
      //  ws.DefaultSiteId = dto.DefaultSiteId;
        ws.UpdatedUtc = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return ws;
    }

    public async Task ArchiveWorkspaceAsync(Guid workspaceId, CancellationToken ct = default)
    {
        var ws = await _db.Workspaces.FirstOrDefaultAsync(x => x.Id == workspaceId, ct)
                 ?? throw new InvalidOperationException("Workspace not found.");

        ws.IsActive = false;
        ws.ArchivedUtc = DateTimeOffset.UtcNow;
        ws.UpdatedUtc = DateTimeOffset.UtcNow;

        // Optional: also archive nodes
        await _db.WorkspaceNodes
            .Where(n => n.WorkspaceId == workspaceId && n.IsActive)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsActive, false)
                .SetProperty(n => n.ArchivedUtc, DateTimeOffset.UtcNow)
                .SetProperty(n => n.UpdatedUtc, DateTimeOffset.UtcNow),
                ct);

        await _db.SaveChangesAsync(ct);
    }

    // -------------------------
    // Nodes
    // -------------------------

    
    public async Task<WorkspaceNodeDto> GetNodeAsync(
        Guid accountId,
        Guid workspaceId,
        Guid nodeId,
        CancellationToken ct = default)
    {
        // Optional but recommended: ensure workspace is owned by account
        var wsOk = await _db.Workspaces.AnyAsync(w => w.Id == workspaceId && w.AccountId == accountId, ct);
        if (!wsOk) throw new InvalidOperationException("Workspace not found.");

        var node = await _db.WorkspaceNodes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == nodeId && n.WorkspaceId == workspaceId, ct);

        if (node is null) throw new InvalidOperationException("Node not found.");

        return node.ToDto();
    }
    
    
    public async Task<WorkspaceNode> CreateNodeAsync(
        Guid accountId,
        Guid workspaceId,
        WorkspaceNodeCreateDto dto,
        CancellationToken ct = default)
    {
        // validate workspace exists
        var wsExists = await _db.Workspaces.AnyAsync(w => w.Id == dto.WorkspaceId, ct);
        if (!wsExists) throw new InvalidOperationException("Workspace not found.");

        // validate parent belongs to same workspace (if provided)
        if (dto.ParentId != null)
        {
            var parentOk = await _db.WorkspaceNodes.AnyAsync(
                n => n.Id == dto.ParentId && n.WorkspaceId == dto.WorkspaceId, ct);

            if (!parentOk) throw new InvalidOperationException("Parent node not found in this workspace.");
        }

        var code = await EnsureUniqueNodeCodeAsync(
            accountId,
            dto.WorkspaceId,
            dto.ParentId,
            dto.Code ?? Slug(dto.Name),
            excludeNodeId: null,
            ct);

        var node = new WorkspaceNode
        {
            Id = Guid.NewGuid(),
            WorkspaceId = dto.WorkspaceId,
            ParentId = dto.ParentId,
            Type = dto.Type,
            IconType = dto.IconType,
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            Code = code,
            SortOrder = dto.SortOrder,
            Lat = dto.Lat,
            Lng = dto.Lng,
            TimeZone = dto.TimeZone,

            Address = dto.Address.ToEntity(),
            Contact = dto.Contact.ToEntity(),

            IsActive = true,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        _db.WorkspaceNodes.Add(node);
        await _db.SaveChangesAsync(ct);

        return node;
    }

public async Task<WorkspaceNode> UpdateNodeAsync(
    Guid accountId,
    Guid workspaceId,
    Guid nodeId,
    WorkspaceNodeUpdateDto dto,
    CancellationToken ct = default)
{
    var node = await _db.WorkspaceNodes
                   .FirstOrDefaultAsync(n => n.Id == nodeId, ct)
               ?? throw new InvalidOperationException("Node not found.");

    // if code provided, ensure unique under same parent scope
    if (!string.IsNullOrWhiteSpace(dto.Code))
    {
        var code = await EnsureUniqueNodeCodeAsync(
            accountId,
            node.WorkspaceId,
            node.ParentId,
            Slug(dto.Code),
            excludeNodeId: nodeId,
            ct);

        node.Code = code;
    }

    node.Name = dto.Name.Trim();
    node.Description = dto.Description?.Trim();
    node.Type = dto.Type;
    node.IconType = dto.IconType;
    node.SortOrder = dto.SortOrder;
    node.Lat = dto.Lat;
    node.Lng = dto.Lng;
    node.TimeZone = dto.TimeZone;
    node.IsActive = dto.IsActive;

    // Address: PATCH semantics
    if (dto.Address is not null)
    {
        node.Address ??= new Address();
        dto.Address.ApplyTo(node.Address);
        // if after applying it's all blank, you can auto-null it:
        if (IsEmpty(node.Address)) node.Address = null;
    }

    // Contact: PATCH semantics
    if (dto.Contact is not null)
    {
        node.Contact ??= new ContactDetails();
        dto.Contact.ApplyTo(node.Contact);
        if (IsEmpty(node.Contact)) node.Contact = null;
    }

    node.UpdatedUtc = DateTimeOffset.UtcNow;

    await _db.SaveChangesAsync(ct);
    return node;
}



    public async Task MoveNodeAsync(Guid accountId,Guid workspaceId,Guid nodeId, WorkspaceNodeMoveDto dto, CancellationToken ct = default)
    {
        var node = await _db.WorkspaceNodes.FirstOrDefaultAsync(n => n.Id == nodeId, ct)
                   ?? throw new InvalidOperationException("Node not found.");

        // validate new parent
        if (dto.NewParentId != null)
        {
            var parent = await _db.WorkspaceNodes.FirstOrDefaultAsync(n => n.Id == dto.NewParentId, ct);
            if (parent == null || parent.WorkspaceId != node.WorkspaceId)
                throw new InvalidOperationException("New parent not found in same workspace.");

            // prevent cycles
            var isDescendant = await IsDescendantAsync(nodeId, dto.NewParentId.Value, ct);
            if (isDescendant) throw new InvalidOperationException("Cannot move a node under one of its descendants.");
        }

        // ensure code uniqueness in new parent scope (only if code is set)
        if (!string.IsNullOrWhiteSpace(node.Code))
        {
            node.Code = await EnsureUniqueNodeCodeAsync(accountId,node.WorkspaceId, dto.NewParentId, node.Code, excludeNodeId: nodeId, ct);
        }

        node.ParentId = dto.NewParentId;
        if (dto.NewSortOrder != null) node.SortOrder = dto.NewSortOrder;
        node.UpdatedUtc = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }

    public async Task ArchiveNodeAsync(Guid accountId,Guid nodeId, CancellationToken ct = default)
    {
        var node = await _db.WorkspaceNodes.FirstOrDefaultAsync(n => n.Id == nodeId, ct)
                   ?? throw new InvalidOperationException("Node not found.");

        node.IsActive = false;
        node.ArchivedUtc = DateTimeOffset.UtcNow;
        node.UpdatedUtc = DateTimeOffset.UtcNow;

        // Optional: archive descendants too
        await ArchiveDescendantsAsync(nodeId, ct);

        await _db.SaveChangesAsync(ct);
    }

    // -------------------------
    // Public helpers
    // -------------------------

    public Task<bool> WorkspaceCodeExistsAsync(Guid accountId, string code, CancellationToken ct = default)
        => _db.Workspaces.AnyAsync(w => w.AccountId == accountId && w.Code == code, ct);

    public Task<bool> NodeCodeExistsAsync(Guid accountId,Guid workspaceId, Guid? parentId, string code, Guid? excludeNodeId = null, CancellationToken ct = default)
    {
        var q = _db.WorkspaceNodes.Where(n =>
            n.WorkspaceId == workspaceId &&
            n.ParentId == parentId &&
            n.Code == code);

        if (excludeNodeId != null)
            q = q.Where(n => n.Id != excludeNodeId);

        return q.AnyAsync(ct);
    }

    // -------------------------
    // Private helpers
    // -------------------------

    private async Task<string> EnsureUniqueWorkspaceCodeAsync(Guid accountId, string baseCode, CancellationToken ct)
    {
        var code = Slug(baseCode);
        if (string.IsNullOrWhiteSpace(code)) code = "workspace";

        var i = 0;
        while (await _db.Workspaces.AnyAsync(w => w.AccountId == accountId && w.Code == code, ct))
        {
            i++;
            code = $"{Slug(baseCode)}-{i}";
        }

        return code;
    }

    private async Task<string> EnsureUniqueNodeCodeAsync(Guid accountId,Guid workspaceId, Guid? parentId, string baseCode, Guid? excludeNodeId, CancellationToken ct)
    {
        var code = Slug(baseCode);
        if (string.IsNullOrWhiteSpace(code)) code = "node";

        var i = 0;
        while (await NodeCodeExistsAsync( accountId,workspaceId, parentId, code, excludeNodeId, ct))
        {
            i++;
            code = $"{Slug(baseCode)}-{i}";
        }

        return code;
    }

    private static string Slug(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        var s = input.Trim().ToLowerInvariant();

        // basic slug: keep letters/digits, replace spaces with '-'
        var chars = s.Select(c => char.IsLetterOrDigit(c) ? c : (char.IsWhiteSpace(c) || c == '-' ? '-' : '\0'))
                     .Where(c => c != '\0')
                     .ToArray();

        var slug = new string(chars);
        while (slug.Contains("--")) slug = slug.Replace("--", "-");
        return slug.Trim('-');
    }

    private async Task<bool> IsDescendantAsync(Guid nodeId, Guid potentialParentId, CancellationToken ct)
    {
        // Walk up from parent → root; if we hit nodeId, then parent is inside node subtree.
        var current = potentialParentId;

        while (true)
        {
            if (current == nodeId) return true;

            var parent = await _db.WorkspaceNodes
                .Where(n => n.Id == current)
                .Select(n => n.ParentId)
                .FirstOrDefaultAsync(ct);

            if (parent == null) return false;

            current = parent.Value;
        }
    }

    private async Task ArchiveDescendantsAsync(Guid rootNodeId, CancellationToken ct)
    {
        // Simple iterative approach (OK for moderate trees)
        var toVisit = new Queue<Guid>();
        toVisit.Enqueue(rootNodeId);

        while (toVisit.Count > 0)
        {
            var id = toVisit.Dequeue();
            var children = await _db.WorkspaceNodes
                .Where(n => n.ParentId == id && n.IsActive)
                .Select(n => n.Id)
                .ToListAsync(ct);

            foreach (var c in children) toVisit.Enqueue(c);

            await _db.WorkspaceNodes
                .Where(n => n.ParentId == id && n.IsActive)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(n => n.IsActive, false)
                    .SetProperty(n => n.ArchivedUtc, DateTimeOffset.UtcNow)
                    .SetProperty(n => n.UpdatedUtc, DateTimeOffset.UtcNow),
                    ct);
        }
    }
    
    private static bool IsEmpty(Address a) =>
        string.IsNullOrWhiteSpace(a.Line1) &&
        string.IsNullOrWhiteSpace(a.Line2) &&
        string.IsNullOrWhiteSpace(a.City) &&
        string.IsNullOrWhiteSpace(a.Province) &&
        string.IsNullOrWhiteSpace(a.PostalCode) &&
        string.IsNullOrWhiteSpace(a.Country);

    private static bool IsEmpty(ContactDetails c) =>
        string.IsNullOrWhiteSpace(c.ContactName) &&
        string.IsNullOrWhiteSpace(c.Phone) &&
        string.IsNullOrWhiteSpace(c.Email);
}