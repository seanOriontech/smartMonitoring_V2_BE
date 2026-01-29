using smartMonitoringBE.ENUMS;

namespace smartMonitoringBE.Models.DTO.Account;

public sealed class PatchBrandingRequest
{
    public BrandingAssetType AssetType { get; set; }
    public string BlobUrl { get; set; } = "";
    public string BlobName  { get; set; } = "";
    public string? ContentType { get; set; }
}