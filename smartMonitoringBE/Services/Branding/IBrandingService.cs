using smartMonitoringBE.Models.DTO.Account;
using smartMonitoringBE.Models.DTO.Branding;

namespace smartMonitoringBE.Services.Branding;

public interface IBrandingService
{
    Task<BrandingDto> GetBrandingAsync(Guid accountId, CancellationToken ct);
    
    Task<BrandingDto> SetBrandingAssetAsync(
        Guid accountId,
        PatchBrandingRequest req,
        CancellationToken ct);
}
