using smartMonitoringBE.ENUMS;

namespace smartMonitoringBE.Models.DTO.Branding;


public sealed class BrandingDto
{
    public string LogoLightUrl { get; set; } = "";
    public string LogoDarkUrl  { get; set; } = "";
    public string IconLightUrl { get; set; } = "";
    public string IconDarkUrl  { get; set; } = "";

    public bool UsingCustomBranding { get; set; }
    
    public bool CustomBrandingIsAvailable { get; set; }
}

public sealed class CreateBrandingUploadRequest
{
    public BrandingAssetType AssetType { get; set; }

    // filename is optional, but helpful for extension guessing
    public string? FileName { get; set; }

    // required: "image/png", "image/svg+xml", "image/webp" etc.
    public string ContentType { get; set; } = "";
}

public class CreateBrandingUploadResponse
{
    public BrandingAssetType AssetType { get; init; }

    /// <summary>
    /// Final blob URL (where the asset will live once committed).
    /// Useful for preview + caching, but not yet written to DB.
    /// </summary>
    public string BlobUrl { get; init; } = string.Empty;

    /// <summary>
    /// Temporary blob name used during upload before commit.
    /// Used for tracking + cleanup.
    /// </summary>
    public string PendingBlobName { get; init; } = string.Empty;

    /// <summary>
    /// SAS upload URL for the client to upload directly to Azure Blob.
    /// </summary>
    public string UploadUrl { get; init; } = string.Empty;

    /// <summary>
    /// Expiry time of the SAS URL.
    /// </summary>
    public DateTimeOffset ExpiresUtc { get; init; }
}