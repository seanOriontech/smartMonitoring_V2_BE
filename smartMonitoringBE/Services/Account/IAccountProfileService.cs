using smartMonitoringBE.Models.DTO.Account;

namespace smartMonitoringBE.Services.Account;

// Features/Accounts/IAccountProfileService.cs
public interface IAccountProfileService
{
    Task<AccountProfileDto> GetAsync(Guid accountId, CancellationToken ct);
    Task<AccountProfileDto> PatchAsync(Guid accountId, UpdateAccountProfileRequest req, CancellationToken ct);
}