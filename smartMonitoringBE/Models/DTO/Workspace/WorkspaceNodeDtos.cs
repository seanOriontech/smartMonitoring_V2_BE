

using smartMonitoringBE.Domain.Entitities.Structure;

namespace smartMonitoringBE.Models.DTO.Workspace;

public sealed record WorkspaceNodeAddressDto(
    string? Line1,
    string? Line2,
    string? City,
    string? Province,
    string? PostalCode,
    string? Country // "ZA" etc
);

public sealed record WorkspaceNodeContactDto(
    string? ContactName,
    string? Phone,
    string? Email
);

public sealed record WorkspaceNodeDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? ParentId,
    WorkspaceNodeType Type,
    NodeIconType IconType,
    string Name,
    string? Code,
    int SortOrder,
    bool IsActive,
    string? Description,
    double? Lat,
    double? Lng,
    string? TimeZone,
    WorkspaceNodeAddressDto? Address,
    WorkspaceNodeContactDto? Contact
);

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
    string? TimeZone,
    WorkspaceNodeAddressDto? Address,
    WorkspaceNodeContactDto? Contact
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
    string? TimeZone,
    WorkspaceNodeAddressDto? Address,
    WorkspaceNodeContactDto? Contact
);

public sealed record WorkspaceNodeMoveDto(
    Guid NodeId,
    Guid? NewParentId,
    int NewSortOrder
);

