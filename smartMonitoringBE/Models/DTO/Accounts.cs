namespace smartMonitoringBE.Models.DTO;


public enum GroupScopeTargetType
{
    Workspace = 1,
    Node = 2
}

public sealed class AccountGroupListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = "";

    public int ScopeCount { get; set; }
    public int UserCount { get; set; }

    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public sealed class GroupScopeDto
{
    public Guid Id { get; set; }

    public GroupScopeTargetType TargetType { get; set; }

    public Guid WorkspaceId { get; set; }
    public Guid? WorkspaceNodeId { get; set; }

    public bool IncludeDescendants { get; set; } = true;
}

public sealed class AccountGroupDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }

    public string Name { get; set; } = "";

    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = "";

    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }

    public List<GroupScopeDto> Scopes { get; set; } = new();
}

public sealed class CreateGroupRequest
{
    public string Name { get; set; } = "";
    public Guid RoleId { get; set; }
    public List<CreateOrReplaceGroupScopeItem> Scopes { get; set; } = new();
}

public sealed class UpdateGroupRequest
{
    public string? Name { get; set; }
    public Guid? RoleId { get; set; }
    // Optional: let you patch scopes in same call, or ignore and use ReplaceScopes endpoint
    public List<CreateOrReplaceGroupScopeItem>? Scopes { get; set; }
}

public sealed class ReplaceGroupScopesRequest
{
    public List<CreateOrReplaceGroupScopeItem> Scopes { get; set; } = new();
}

public sealed class CreateOrReplaceGroupScopeItem
{
    public GroupScopeTargetType TargetType { get; set; }

    public Guid WorkspaceId { get; set; }
    public Guid? WorkspaceNodeId { get; set; }

    public bool IncludeDescendants { get; set; } = true;
}