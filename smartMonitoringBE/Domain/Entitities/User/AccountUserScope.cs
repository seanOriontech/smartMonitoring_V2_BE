using smartMonitoringBE.Domain.Entitities.Structure;

namespace smartMonitoringBE.Domain.Entitities.User;

public enum ScopeTargetType
{
    Workspace = 1,
    Node = 2
}

public sealed class AccountUserScope
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }
    public Guid UserId { get; set; }
    public AccountUser AccountUser { get; set; } = null!;

    public ScopeTargetType TargetType { get; set; }

    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    public Guid? WorkspaceNodeId { get; set; }
    public WorkspaceNode? WorkspaceNode { get; set; }

    public bool IncludeDescendants { get; set; } = true;

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
}