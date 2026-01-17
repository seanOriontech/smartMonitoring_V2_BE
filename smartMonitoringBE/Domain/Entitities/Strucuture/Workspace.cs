using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Domain.Entitities.Structure;

public sealed class Workspace
{
    public Guid Id { get; set; }

    // parent tenant
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public WorkspaceType Type { get; set; } = WorkspaceType.Organisation;

    // Common
    public string Name { get; set; } = "";
    public string? Description { get; set; } // useful for Project, optional otherwise

    // Human-friendly key for URLs and imports (unique per account)
    public string Code { get; set; } = ""; // e.g. "oriontech", "client-acme", "home"

    // Location/contact (useful for Organisation/Client, optional for Project)
    public Address? Address { get; set; }
    public ContactDetails? Contact { get; set; }

    // Optional defaults for easy UX
    public Guid? DefaultSiteId { get; set; }

    public string? TimeZone { get; set; }

    public bool PrimaryWorkspace { get; set; }

    // Future-proofing
    public string? TagsJson { get; set; } // ["cold-room","critical"]
    public string? MetadataJson { get; set; } // arbitrary key/values

    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }
    public DateTimeOffset? ArchivedUtc { get; set; }

    public ICollection<WorkspaceNode> Nodes { get; set; } = new List<WorkspaceNode>();

}

