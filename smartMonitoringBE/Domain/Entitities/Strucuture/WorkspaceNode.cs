namespace smartMonitoringBE.Domain.Entitities.Structure;

public sealed class WorkspaceNode
{
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    // Tree
    public Guid? ParentId { get; set; }
    public WorkspaceNode? Parent { get; set; }
    public ICollection<WorkspaceNode> Children { get; set; } = new List<WorkspaceNode>();

    // Type
    public WorkspaceNodeType Type { get; set; } = WorkspaceNodeType.Folder;

    public NodeIconType IconType { get; set; } = NodeIconType.Default;
    // Display
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    // Optional friendly key within workspace (URLs / integrations)
    public string? Code { get; set; }

    // Optional location/contact (used for Site/POI/Area if needed)
    public double? Lat { get; set; }
    public double? Lng { get; set; }

    public Address? Address { get; set; }
    

    
    public ContactDetails? Contact { get; set; }

    // Optional per-node timezone override (IANA e.g. "Africa/Johannesburg")
    public string? TimeZone { get; set; }

    // UX + extensibility
    public int SortOrder { get; set; } = 0;
    public string? TagsJson { get; set; }
    public string? MetadataJson { get; set; }

    // Lifecycle
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }
    public DateTimeOffset? ArchivedUtc { get; set; }
}