
using smartMonitoringBE.Domain.Entitities.Structure;
using smartMonitoringBE.Models.DTO.Workspace;

namespace smartMonitoringBE.Services.Workspaces;

public interface IWorkspaceService
{
    // Workspaces
    Task<Workspace> CreateWorkspaceAsync(Guid AccountId,WorkspaceCreateDto dto, CancellationToken ct = default);
    Task<Workspace> UpdateWorkspaceAsync(Guid accountId,Guid workspaceId, WorkspaceUpdateDto dto, CancellationToken ct = default);
    //Task ArchiveWorkspaceAsync(Guid workspaceId, CancellationToken ct = default); // “delete” = soft delete

    // Nodes
    Task<WorkspaceNode> CreateNodeAsync(Guid accountId,Guid workspaceId, WorkspaceNodeCreateDto dto, CancellationToken ct = default);
    Task<WorkspaceNode> UpdateNodeAsync(Guid accountId,Guid workspaceId, Guid nodeId, WorkspaceNodeUpdateDto dto, CancellationToken ct = default);
    Task MoveNodeAsync(Guid accountId,Guid workspaceId ,Guid nodeId, WorkspaceNodeMoveDto dto, CancellationToken ct = default);
    Task ArchiveNodeAsync(Guid accountId,Guid nodeId, CancellationToken ct = default); // soft delete

    // Optional helpers
    Task<bool> WorkspaceCodeExistsAsync(Guid accountId, string code, CancellationToken ct = default);
    Task<bool> NodeCodeExistsAsync(Guid accountId,Guid workspaceId, Guid? parentId, string code, Guid? excludeNodeId = null, CancellationToken ct = default);
}