namespace smartMonitoringBE.Models.DTO.Security;



public sealed class RolePermissionsDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = "";
    public bool IsSystem { get; set; }

    public List<PermissionDto> Permissions { get; set; } = new(); // all available
    public List<string> SelectedCodes { get; set; } = new();      // which are enabled
}

public sealed class UpdateRolePermissionsRequest
{
    // "replace list" approach (simple + safe)
    public List<string> PermissionCodes { get; set; } = new();
}