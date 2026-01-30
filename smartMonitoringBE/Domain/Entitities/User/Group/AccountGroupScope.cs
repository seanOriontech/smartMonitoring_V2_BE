using smartMonitoringBE.Domain.Entitities.Structure;

namespace smartMonitoringBE.Domain.Entitities.User.Group;

public sealed class AccountGroupScope
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }
    public Guid GroupId { get; set; }
    public AccountGroup Group { get; set; } = null!;

    public ScopeTargetType TargetType { get; set; } // Workspace or Node

    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    public Guid? WorkspaceNodeId { get; set; }
    public WorkspaceNode? WorkspaceNode { get; set; }

    public bool IncludeDescendants { get; set; } = true;

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
}