using smartMonitoringBE.Domain.Entitities.Structure;

namespace smartMonitoringBE.Models.DTO.Workspace;

public sealed record WorkspaceCreateDto(

    string Name,
    string? Code,
    string? Description,
    string? TimeZone,
  WorkspaceType Type ,
    bool PrimaryWorkspace
);

public sealed record WorkspaceUpdateDto(
    string Name,
    string? Description,
    string? TimeZone,
    WorkspaceType workspaceType ,
    bool PrimaryWorkspace,
    bool IsActive
);

public sealed record WorkspaceDto(
    Guid Id,
    Guid AccountId,
    string Name,
    string Code,
    string? Description,
    string? TimeZone,
    bool PrimaryWorkspace,
    bool IsActive,
    DateTimeOffset CreatedUtc,
    DateTimeOffset? UpdatedUtc
);