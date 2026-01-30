using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.Models.DTO.Security;
using smartMonitoringBE.Services.Security;


namespace smartMonitoringBE.Controllers;

[ApiController]
[Route("api/accounts/{accountId:guid}/roles")]
public sealed class RolePermissionsController : ControllerBase
{
    private readonly IRolePermissionService _svc;

    public RolePermissionsController(IRolePermissionService svc)
    {
        _svc = svc;
    }

    // GET: api/accounts/{accountId}/roles/{roleId}/permissions
    [HttpGet("{roleId:guid}/permissions")]
    public async Task<ActionResult<RolePermissionsDto>> GetRolePermissions(Guid accountId, Guid roleId, CancellationToken ct)
    {
        var dto = await _svc.GetRolePermissionsAsync(accountId, roleId, ct);
        return Ok(dto);
    }

    // PUT: api/accounts/{accountId}/roles/{roleId}/permissions
    [HttpPut("{roleId:guid}/permissions")]
    public async Task<ActionResult<RolePermissionsDto>> ReplaceRolePermissions(
        Guid accountId,
        Guid roleId,
        [FromBody] UpdateRolePermissionsRequest req,
        CancellationToken ct)
    {
        var dto = await _svc.ReplaceRolePermissionsAsync(accountId, roleId, req, ct);
        return Ok(dto);
    }
}