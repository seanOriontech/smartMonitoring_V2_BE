using smartMonitoringBE.Models.DTO.Workspace;

namespace smartMonitoringBE.Services.Workspaces;

public interface IWorkspaceTreeService
{
    Task<WorkspaceTreeResponseDto> GetWorkspaceTreeAsync(Guid accountId, CancellationToken ct);
}