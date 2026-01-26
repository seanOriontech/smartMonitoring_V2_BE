using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Security;


public sealed class AuthorizationContextLoader : IAuthorizationContextLoader
{
    private readonly SmartMonitoringDbContext _db;

    public AuthorizationContextLoader(SmartMonitoringDbContext db) => _db = db;

    public async Task<AuthzContext?> Load(Guid userId, Guid accountId, CancellationToken ct)
    {
        var data = await _db.AccountUsers
            .AsNoTracking()
            .Where(au => au.UserId == userId && au.AccountId == accountId)
            .Select(au => new
            {
                au.AccountId,
                au.RoleId,
                PermissionCodes = au.Role.Permissions
                    .Select(rp => rp.Permission.Code)
                    .ToList(),
                Scopes = au.Scopes.Select(s => new
                {
                    s.TargetType,      // your enum
                    s.WorkspaceId,
                    s.WorkspaceNodeId,
                    s.AccountUser.Role.Id
                }).ToList()
            })
            .SingleOrDefaultAsync(ct);

        if (data is null) return null;

        var perms = data.PermissionCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var hasAll = data.Scopes.Any(s => s.Id  == new Guid("a1000000-0000-0000-0000-000000000001")); // The user is a owner has access to everything
        var workspaceIds = data.Scopes.Where(s => s.WorkspaceId != null)
            .Select(s => s.WorkspaceId).ToHashSet();

        var nodeIds = data.Scopes.Where(s => s.WorkspaceNodeId != null)
            .Select(s => s.WorkspaceNodeId!.Value).ToHashSet();

        return new AuthzContext(
            userId,
            data.AccountId,
            data.RoleId,
            perms,
            new ScopeSet(hasAll, workspaceIds, nodeIds)
        );
    }
}