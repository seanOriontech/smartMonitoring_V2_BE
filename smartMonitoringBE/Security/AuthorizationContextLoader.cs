using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using smartMonitoringBE.Infrastructure.Constants;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Security;



public sealed class AuthorizationContextLoader : IAuthorizationContextLoader
{
    private readonly SmartMonitoringDbContext _db;

    public AuthorizationContextLoader(SmartMonitoringDbContext db) => _db = db;

    public async Task<AuthzContext?> Load(Guid userId, Guid accountId, CancellationToken ct)
    {
        var userOid = userId.ToString();

        var data = await _db.AccountUsers
            .AsNoTracking()
            .Where(au => au.User.ObjectId == userOid && au.AccountId == accountId)
            .Select(au => new
            {
                au.AccountId,
                au.RoleId,
                PermissionCodes = au.Role.Permissions
                    .Select(rp => rp.Permission.Code)
                    .ToList(),
                Scopes = au.Scopes.Select(s => new
                {
                    s.TargetType,
                    s.WorkspaceId,
                    s.WorkspaceNodeId,
                    RoleId = au.RoleId // <-- don’t do s.AccountUser.Role.Id here
                }).ToList()
            })
            .SingleOrDefaultAsync(ct);

        if (data is null) return null;

        var perms = data.PermissionCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var isOwner = data.RoleId == RoleConstants.Owner;

// Owner automatically has full access
        var hasAll = isOwner || data.Scopes.Count == 0;// The user is a owner has access to everything
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