using smartMonitoringBE.ENUMS;
using smartMonitoringBE.Models.DTO.Branding;

namespace smartMonitoringBE.Services.Branding;

public interface IAccountBrandingBlobService
{
    Task<CreateBrandingUploadResponse> CreateUploadAsync(
        Guid accountId,
        CreateBrandingUploadRequest req,
        CancellationToken ct);

    Task DeleteAsync(Guid accountId, BrandingAssetType assetType, CancellationToken ct);
}