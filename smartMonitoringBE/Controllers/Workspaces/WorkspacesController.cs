using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using smartMonitoringBE.Domain.Entitities.Structure;
using smartMonitoringBE.Models.DTO.Workspace;
using smartMonitoringBE.Services.Workspaces;

namespace smartMonitoringBE.Api.Controllers;

[ApiController]
[Route("api/accounts/{accountId:guid}/workspaces")]
public sealed class WorkspacesController : ControllerBase
{
    private readonly IWorkspaceService _svc;
    private readonly IWorkspaceTreeService _svcr;

    public WorkspacesController(IWorkspaceService svc, IWorkspaceTreeService svcr)
    {
        _svc = svc;
        _svcr = svcr;
    }



    [HttpPost]
    public async Task<IActionResult> Create([FromRoute] Guid accountId, [FromBody] WorkspaceCreateDto req, CancellationToken ct)
    {
        var created = await _svc.CreateWorkspaceAsync(accountId, req, ct);
        return CreatedAtAction(nameof(GetOne), new { accountId, workspaceId = created.Id }, created);
    }

    [HttpPatch("{workspaceId:guid}")]
  //  [Authorize(Policy = "WorkspaceAccess")]
    public async Task<IActionResult> Update([FromRoute] Guid accountId, [FromRoute] Guid workspaceId, [FromBody] WorkspaceUpdateDto req, CancellationToken ct)
        => Ok(await _svc.UpdateWorkspaceAsync(accountId, workspaceId, req, ct));
    
    
    [HttpGet("tree")]
  //  [Authorize(Policy = "WorkspaceAccess")]
    public async Task<IActionResult> GetOne(Guid accountId, CancellationToken ct)
        => Ok(await _svcr.GetWorkspaceTreeAsync(accountId, ct));

    [HttpDelete("{workspaceId:guid}")]
   
    public async Task<IActionResult> Archive( [FromRoute] Guid workspaceId, CancellationToken ct)
    {
        await _svc.ArchiveWorkspaceAsync( workspaceId, ct);
        return NoContent();
    }
}