namespace smartMonitoringBE.Security;


public sealed record AuthzContext(
    Guid UserId,
    Guid AccountId,
    Guid RoleId,
    HashSet<string> PermissionCodes,
    ScopeSet Scopes
);

public sealed record ScopeSet(
    bool HasAllWorkspaces,
    HashSet<Guid> WorkspaceIds,
    HashSet<Guid> NodeIds
);