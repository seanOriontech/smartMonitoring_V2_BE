

using smartMonitoringBE.Domain.Entitities.Structure;

namespace smartMonitoringBE.Models.DTO.Workspace;

public sealed record WorkspaceNodeCreateDto(
    Guid WorkspaceId,
    Guid? ParentId,
    WorkspaceNodeType Type,
    string Name,
    string? Code,
    NodeIconType IconType,
    int SortOrder,
    string? Description,
    double? Lat,
    double? Lng,
    string? TimeZone
);

public sealed record WorkspaceNodeUpdateDto(
    WorkspaceNodeType Type,
    string Name,

    string? Code,
    NodeIconType IconType,
    int SortOrder,
    bool IsActive,
    string? Description,
    double? Lat,
    double? Lng,
    string? TimeZone
);

public sealed record WorkspaceNodeMoveDto(
    Guid NodeId,
    Guid? NewParentId,
    int NewSortOrder
);

public sealed record WorkspaceNodeDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? ParentId,
    WorkspaceNodeType Type,
    NodeIconType IconType,
    string Name,
    string? Description,
    string? Code,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedUtc,
    DateTimeOffset? UpdatedUtc
);