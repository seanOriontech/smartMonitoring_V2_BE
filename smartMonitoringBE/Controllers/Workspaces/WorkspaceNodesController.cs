using Microsoft.AspNetCore.Mvc;

using smartMonitoringBE.Domain.Entitities.Structure;
using smartMonitoringBE.Models.DTO.Workspace;
using smartMonitoringBE.Services.Workspaces;

namespace smartMonitoringBE.Api.Controllers;

[ApiController]
[Route("api/accounts/{accountId:guid}/workspaces/{workspaceId:guid}/nodes")]

public sealed class WorkspaceNodesController : ControllerBase
{
    private readonly IWorkspaceService _svc;
    private readonly IWorkspaceTreeService _svcr;

    public WorkspaceNodesController(IWorkspaceService svc, IWorkspaceTreeService svcr)
    {
        _svc = svc;
        _svcr = svcr;
    }

    [HttpGet]
    public async Task<IActionResult> GetTree([FromRoute] Guid accountId, [FromRoute] Guid workspaceId, CancellationToken ct)
        => Ok(await _svcr.GetWorkspaceTreeAsync(accountId, ct));

    [HttpPost]
    public async Task<IActionResult> CreateNode([FromRoute] Guid accountId, [FromRoute] Guid workspaceId, [FromBody]  WorkspaceNodeCreateDto req, CancellationToken ct)
        => Ok(await _svc.CreateNodeAsync(accountId, workspaceId, req, ct));

    [HttpPatch("{nodeId:guid}")]
    public async Task<IActionResult> UpdateNode([FromRoute] Guid accountId, [FromRoute] Guid workspaceId, [FromRoute] Guid nodeId, [FromBody] WorkspaceNodeUpdateDto req, CancellationToken ct)
        => Ok(await _svc.UpdateNodeAsync(accountId, workspaceId, nodeId, req, ct));

    [HttpPost("{nodeId:guid}/move")]
    public async Task<IActionResult> MoveNode([FromRoute] Guid accountId, [FromRoute] Guid workspaceId, [FromRoute] Guid nodeId, [FromBody] WorkspaceNodeMoveDto req, CancellationToken ct)
        => Ok();
}