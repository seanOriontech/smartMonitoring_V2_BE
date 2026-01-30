using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Domain.Entitities.User;
using smartMonitoringBE.Domain.Entitities.User.Group;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO;

namespace smartMonitoringBE.Services.Account;

public sealed class AccountGroupService : IAccountGroupService
{
    private readonly SmartMonitoringDbContext _db;

    public AccountGroupService(SmartMonitoringDbContext db)
    {
        _db = db;
    }

    public async Task<List<AccountGroupListItemDto>> GetGroupsAsync(Guid accountId, CancellationToken ct)
    {
        // We do a single query with counts.
        var items = await _db.AccountGroups
            .AsNoTracking()
            .Where(g => g.AccountId == accountId && g.IsActive)
            .Select(g => new AccountGroupListItemDto
            {
                Id = g.Id,
                Name = g.Name,
                RoleId = g.RoleId,
                RoleName = g.Role.Name,
                ScopeCount = g.Scopes.Count,
                UserCount = g.Users.Count,
                IsDefault = g.IsDefault,
                IsActive = g.IsActive
            })
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);

        return items;
    }

    public async Task<AccountGroupDto> GetGroupAsync(Guid accountId, Guid groupId, CancellationToken ct)
    {
        var dto = await _db.AccountGroups
            .AsNoTracking()
            .Where(g => g.AccountId == accountId && g.Id == groupId)
            .Select(g => new AccountGroupDto
            {
                Id = g.Id,
                AccountId = g.AccountId,
                Name = g.Name,
                RoleId = g.RoleId,
                RoleName = g.Role.Name,
                IsDefault = g.IsDefault,
                IsActive = g.IsActive,
                Scopes = g.Scopes
                    .OrderBy(s => s.TargetType)
                    .ThenBy(s => s.WorkspaceId)
                    .ThenBy(s => s.WorkspaceNodeId)
                    .Select(s => new GroupScopeDto
                    {
                        Id = s.Id,
                        TargetType = (GroupScopeTargetType)s.TargetType,
                        WorkspaceId = s.WorkspaceId,
                        WorkspaceNodeId = s.WorkspaceNodeId,
                        IncludeDescendants = s.IncludeDescendants
                    })
                    .ToList()
            })
            .SingleOrDefaultAsync(ct);

        if (dto is null) throw new KeyNotFoundException("Group not found.");
        return dto;
    }

    public async Task<AccountGroupDto> CreateAsync(Guid accountId, CreateGroupRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new ArgumentException("Name is required.");

        // Ensure account exists (optional but nice)
        var accountExists = await _db.Accounts.AsNoTracking().AnyAsync(a => a.Id == accountId, ct);
        if (!accountExists) throw new KeyNotFoundException("Account not found.");

        // Validate role (system role or same account role)
        await EnsureRoleAllowedAsync(accountId, req.RoleId, ct);

        // Validate scopes belong to this account
        await ValidateScopesAsync(accountId, req.Scopes, ct);

        var now = DateTimeOffset.UtcNow;

        var g = new AccountGroup
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Name = req.Name.Trim(),
            Code = Slug(req.Name), // optional helper, can remove if you want
            RoleId = req.RoleId,
            IsActive = true,
            IsDefault = false,
            CreatedUtc = now,
            UpdatedUtc = now,
        };

        g.Scopes = req.Scopes.Select(s => new AccountGroupScope
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            GroupId = g.Id,
            TargetType = (ScopeTargetType)s.TargetType,
            WorkspaceId = s.WorkspaceId,
            WorkspaceNodeId = s.TargetType == GroupScopeTargetType.Node ? s.WorkspaceNodeId : null,
            IncludeDescendants = s.IncludeDescendants,
            CreatedUtc = now
        }).ToList();

        _db.AccountGroups.Add(g);
        await _db.SaveChangesAsync(ct);

        return await GetGroupAsync(accountId, g.Id, ct);
    }

    public async Task<AccountGroupDto> UpdateAsync(Guid accountId, Guid groupId, UpdateGroupRequest req, CancellationToken ct)
    {
        var g = await _db.AccountGroups
            .Include(x => x.Scopes)
            .SingleOrDefaultAsync(x => x.AccountId == accountId && x.Id == groupId, ct);

        if (g is null) throw new KeyNotFoundException("Group not found.");

        if (req.Name is not null)
        {
            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
            g.Name = name;
            g.Code = Slug(name);
        }

        if (req.RoleId.HasValue)
        {
            await EnsureRoleAllowedAsync(accountId, req.RoleId.Value, ct);
            g.RoleId = req.RoleId.Value;
        }

        if (req.Scopes is not null)
        {
            await ValidateScopesAsync(accountId, req.Scopes, ct);
            ReplaceScopesInternal(g, accountId, req.Scopes);
        }

        g.UpdatedUtc = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        return await GetGroupAsync(accountId, groupId, ct);
    }

    public async Task<AccountGroupDto> ReplaceScopesAsync(Guid accountId, Guid groupId, ReplaceGroupScopesRequest req, CancellationToken ct)
    {
        var g = await _db.AccountGroups
            .Include(x => x.Scopes)
            .SingleOrDefaultAsync(x => x.AccountId == accountId && x.Id == groupId, ct);

        if (g is null) throw new KeyNotFoundException("Group not found.");

        await ValidateScopesAsync(accountId, req.Scopes, ct);

        ReplaceScopesInternal(g, accountId, req.Scopes);
        g.UpdatedUtc = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return await GetGroupAsync(accountId, groupId, ct);
    }

    public async Task DeleteAsync(Guid accountId, Guid groupId, CancellationToken ct)
    {
        var g = await _db.AccountGroups
            .Include(x => x.Users)
            .SingleOrDefaultAsync(x => x.AccountId == accountId && x.Id == groupId, ct);

        if (g is null) return;

        var inUse = g.Users.Any();
        if (inUse)
            throw new InvalidOperationException("This group is assigned to one or more users. Reassign users first.");

        // If you prefer hard delete, Remove. If you prefer soft delete: set IsActive=false.
        _db.AccountGroups.Remove(g);
        await _db.SaveChangesAsync(ct);
    }

    // ---------- helpers ----------

    private async Task EnsureRoleAllowedAsync(Guid accountId, Guid roleId, CancellationToken ct)
    {
        var ok = await _db.Roles
            .AsNoTracking()
            .AnyAsync(r => r.Id == roleId && (r.IsSystem || r.AccountId == accountId) && r.IsActive, ct);

        if (!ok) throw new InvalidOperationException("Role is not available for this account.");
    }

    private async Task ValidateScopesAsync(Guid accountId, List<CreateOrReplaceGroupScopeItem> scopes, CancellationToken ct)
    {
        // v1: allow empty scopes (meaning “no access”) or require at least one — your call.
        if (scopes is null) throw new ArgumentException("Scopes are required.");

        // Basic shape validation
        foreach (var s in scopes)
        {
            if (s.WorkspaceId == Guid.Empty) throw new ArgumentException("WorkspaceId is required in scope.");
            if (s.TargetType == GroupScopeTargetType.Node && s.WorkspaceNodeId is null)
                throw new ArgumentException("WorkspaceNodeId is required when TargetType=Node.");
        }

        // Ensure all workspaces belong to this account
        var wsIds = scopes.Select(x => x.WorkspaceId).Distinct().ToList();

        var existingWs = await _db.Workspaces
            .AsNoTracking()
            .Where(w => w.AccountId == accountId && wsIds.Contains(w.Id))
            .Select(w => w.Id)
            .ToListAsync(ct);

        if (existingWs.Count != wsIds.Count)
            throw new InvalidOperationException("One or more workspaces in scope do not belong to this account.");

        // Ensure nodes belong to the workspace (and account, by joining workspace)
        var nodeItems = scopes.Where(x => x.TargetType == GroupScopeTargetType.Node).ToList();
        if (nodeItems.Count > 0)
        {
            var nodeIds = nodeItems.Select(x => x.WorkspaceNodeId!.Value).Distinct().ToList();

            var existingNodes = await _db.WorkspaceNodes
                .AsNoTracking()
                .Where(n => nodeIds.Contains(n.Id))
                .Select(n => new { n.Id, n.WorkspaceId })
                .ToListAsync(ct);

            if (existingNodes.Count != nodeIds.Count)
                throw new InvalidOperationException("One or more nodes in scope do not exist.");

            // ensure node.WorkspaceId matches scope.WorkspaceId
            var map = existingNodes.ToDictionary(x => x.Id, x => x.WorkspaceId);
            foreach (var it in nodeItems)
            {
                if (!map.TryGetValue(it.WorkspaceNodeId!.Value, out var wsId) || wsId != it.WorkspaceId)
                    throw new InvalidOperationException("One or more nodes do not belong to the specified workspace.");
            }
        }
    }

    private static void ReplaceScopesInternal(AccountGroup g, Guid accountId, List<CreateOrReplaceGroupScopeItem> scopes)
    {
        // Remove old
        g.Scopes.Clear();

        var now = DateTimeOffset.UtcNow;
        foreach (var s in scopes)
        {
            g.Scopes.Add(new AccountGroupScope
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                GroupId =  g.Id,
             
                TargetType = (ScopeTargetType)s.TargetType,
                WorkspaceId = s.WorkspaceId,
                WorkspaceNodeId = s.TargetType == GroupScopeTargetType.Node ? s.WorkspaceNodeId : null,
                IncludeDescendants = s.IncludeDescendants,
                CreatedUtc = now
            });
        }
    }

    private static string Slug(string input)
    {
        input = input.Trim().ToLowerInvariant();
        var chars = input.Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray();
        var s = new string(chars);
        while (s.Contains("--")) s = s.Replace("--", "-");
        return s.Trim('-');
    }
}