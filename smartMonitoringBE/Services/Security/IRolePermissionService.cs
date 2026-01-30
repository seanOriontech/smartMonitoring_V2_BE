using smartMonitoringBE.Models.DTO.Security;

namespace smartMonitoringBE.Services.Security;

public interface IRolePermissionService
{
    Task<RolePermissionsDto> GetRolePermissionsAsync(Guid accountId, Guid roleId, CancellationToken ct);
    Task<RolePermissionsDto> ReplaceRolePermissionsAsync(Guid accountId, Guid roleId, UpdateRolePermissionsRequest req, CancellationToken ct);
}