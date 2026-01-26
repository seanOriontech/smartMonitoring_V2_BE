

using Microsoft.AspNetCore.Authorization;
using smartMonitoringBE.Infrastructure.Data;

namespace smartMonitoringBE.Security.Policies;

public sealed class WorkspaceAccessHandler : AuthorizationHandler<WorkspaceAccessRequirement>
{
    private readonly IAuthzContextAccessor _authz;
    private readonly SmartMonitoringDbContext _db;

    public WorkspaceAccessHandler(IAuthzContextAccessor authz, SmartMonitoringDbContext db)
    {
        _authz = authz;
        _db = db;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkspaceAccessRequirement requirement)
    {
        if (context.Resource is not HttpContext http) return;

        var accountRaw = http.Request.RouteValues["accountId"]?.ToString();
        var workspaceRaw = http.Request.RouteValues["workspaceId"]?.ToString();

        if (!Guid.TryParse(accountRaw, out var accountId)) return;
        if (!Guid.TryParse(workspaceRaw, out var workspaceId)) return;

        var authzCtx = await _authz.GetAsync(accountId, CancellationToken.None);

        var ok = await ScopeChecks.HasWorkspaceAccess(_db, authzCtx, workspaceId, CancellationToken.None);
        if (ok) context.Succeed(requirement);
    }
}