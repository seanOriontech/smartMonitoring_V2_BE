using smartMonitoringBE.Models.DTO;

namespace smartMonitoringBE.Services.Account;



public interface IAccountGroupService
{
    Task<List<AccountGroupListItemDto>> GetGroupsAsync(Guid accountId, CancellationToken ct);
    Task<AccountGroupDto> GetGroupAsync(Guid accountId, Guid groupId, CancellationToken ct);

    Task<AccountGroupDto> CreateAsync(Guid accountId, CreateGroupRequest req, CancellationToken ct);
    Task<AccountGroupDto> UpdateAsync(Guid accountId, Guid groupId, UpdateGroupRequest req, CancellationToken ct);

    Task<AccountGroupDto> ReplaceScopesAsync(Guid accountId, Guid groupId, ReplaceGroupScopesRequest req, CancellationToken ct);

    Task DeleteAsync(Guid accountId, Guid groupId, CancellationToken ct);
}