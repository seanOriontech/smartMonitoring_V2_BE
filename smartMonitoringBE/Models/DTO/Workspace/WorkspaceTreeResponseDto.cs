using smartMonitoringBE.Domain.Entitities.Structure;

namespace smartMonitoringBE.Models.DTO.Workspace;

public sealed class WorkspaceTreeResponseDto
{
    public Guid AccountId { get; set; }
    public List<WorkspaceTreeDto> Workspaces { get; set; } = new();
}

public sealed class WorkspaceTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public WorkspaceType Type { get; set; }
    
    public string? TimeZone { get; set; }
    
    public string? Description { get; set; }
    public List<WorkspaceNodeTreeDto> Nodes { get; set; } = new();
}

public sealed class WorkspaceNodeTreeDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = "";
    public string? Code { get; set; }
    public WorkspaceNodeType Type { get; set; }
    public NodeIconType IconType { get; set; }
    public int SortOrder { get; set; }

    public List<WorkspaceNodeTreeDto> Children { get; set; } = new();
}