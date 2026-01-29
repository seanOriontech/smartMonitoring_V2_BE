using smartMonitoringBE.Models.DTO.Account;
using smartMonitoringBE.Models.Requests.Account;

namespace smartMonitoringBE.Services.Account;

public interface IAccountThemeService
{
    Task<AccountThemeDto> GetThemeAsync(Guid accountId, CancellationToken ct);
    Task<AccountThemeDto> PatchThemeAsync(Guid accountId, PatchAccountThemeRequest req, CancellationToken ct);
}