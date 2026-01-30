using Microsoft.AspNetCore.Mvc;
using smartMonitoringBE.Models.DTO.Security;
using smartMonitoringBE.Services.Security;

namespace smartMonitoringBE.Controllers.Security;



[ApiController]
[Route("api/accounts/{accountId:guid}/roles")]
public sealed class RolesController : ControllerBase
{
    private readonly IRoleService _svc;

    public RolesController(IRoleService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoleListItemDto>>> GetRoles(Guid accountId, CancellationToken ct)
        => Ok(await _svc.GetRolesAsync(accountId, ct));

    [HttpGet("{roleId:guid}")]
    public async Task<ActionResult<RoleDetailDto>> GetRole(Guid accountId, Guid roleId, CancellationToken ct)
        => Ok(await _svc.GetRoleAsync(accountId, roleId, ct));

    [HttpPost]
    public async Task<ActionResult<RoleDetailDto>> CreateRole(Guid accountId, [FromBody] CreateRoleRequest req, CancellationToken ct)
        => Ok(await _svc.CreateRoleAsync(accountId, req, ct));

    [HttpPatch("{roleId:guid}")]
    public async Task<ActionResult<RoleDetailDto>> PatchRole(Guid accountId, Guid roleId, [FromBody] PatchRoleRequest req, CancellationToken ct)
        => Ok(await _svc.PatchRoleAsync(accountId, roleId, req, ct));

  

    [HttpDelete("{roleId:guid}")]
    public async Task<IActionResult> DeleteRole(Guid accountId, Guid roleId, CancellationToken ct)
    {
        await _svc.DeleteRoleAsync(accountId, roleId, ct);
        return NoContent();
    }
}