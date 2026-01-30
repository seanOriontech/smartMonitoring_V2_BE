using smartMonitoringBE.Models.DTO.Security;

namespace smartMonitoringBE.Services.Security;

public interface IRoleService
{
    Task<List<RoleListItemDto>> GetRolesAsync(Guid accountId, CancellationToken ct);
    Task<RoleDetailDto> GetRoleAsync(Guid accountId, Guid roleId, CancellationToken ct);

    Task<RoleDetailDto> CreateRoleAsync(Guid accountId, CreateRoleRequest req, CancellationToken ct);
    Task<RoleDetailDto> PatchRoleAsync(Guid accountId, Guid roleId, PatchRoleRequest req, CancellationToken ct);

    Task<RoleDetailDto> ReplacePermissionsAsync(Guid accountId, Guid roleId, ReplaceRolePermissionsRequest req, CancellationToken ct);

    Task DeleteRoleAsync(Guid accountId, Guid roleId, CancellationToken ct);
}