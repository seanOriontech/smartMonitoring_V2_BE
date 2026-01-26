

using Microsoft.AspNetCore.Authorization;


namespace smartMonitoringBE.Security.Policies;

public sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IAuthzContextAccessor _authz;

    public PermissionHandler(IAuthzContextAccessor authz) => _authz = authz;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // accountId must come from route or header (recommended route)
        if (!TryGetAccountId(context, out var accountId)) return;

        var authzCtx = await _authz.GetAsync(accountId, CancellationToken.None);

        if (authzCtx.PermissionCodes.Contains(requirement.PermissionCode))
            context.Succeed(requirement);
    }

    private static bool TryGetAccountId(AuthorizationHandlerContext ctx, out Guid accountId)
    {
        accountId = default;
        if (ctx.Resource is not HttpContext http) return false;

        var raw = http.Request.RouteValues.TryGetValue("accountId", out var v) ? v?.ToString() : null;
        return Guid.TryParse(raw, out accountId);
    }
}