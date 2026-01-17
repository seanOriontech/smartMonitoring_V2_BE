using smartMonitoringBE.Models.DTO.Tiers;

namespace smartMonitoringBE.Services.Tiers;



public interface ITiersService
{
    Task<TierCatalogDto> GetLatestAsync(CancellationToken ct);
}