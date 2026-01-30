namespace smartMonitoringBE.Models.DTO.Security;



public sealed class PermissionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = "";
    public string Group { get; set; } = "";
    public string? Description { get; set; }
}

public sealed class RoleListItemDto
{
    public Guid Id { get; set; }
    public Guid? AccountId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public int PermissionCount { get; set; }
}

public sealed class RoleDetailDto
{
    public Guid Id { get; set; }
    public Guid? AccountId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
}

// Requests
public sealed class CreateRoleRequest
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public List<Guid>? PermissionIds { get; set; }
}

public sealed class PatchRoleRequest
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}

public sealed class ReplaceRolePermissionsRequest
{
    public List<Guid> PermissionIds { get; set; } = new();
}