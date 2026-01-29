using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using smartMonitoringBE.ENUMS;
using smartMonitoringBE.Models.DTO.Branding;
using smartMonitoringBE.Models.DTO.Options;

namespace smartMonitoringBE.Services.Branding;

public sealed class AccountBrandingBlobService : IAccountBrandingBlobService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png",
        "image/jpeg",
        "image/webp",
        "image/svg+xml"
    };

    private readonly BlobServiceClient _blobSvc;
    private readonly AccountBrandingStorageOptions _opt;

    public AccountBrandingBlobService(
        BlobServiceClient blobSvc,
        IOptions<AccountBrandingStorageOptions> opt)
    {
        _blobSvc = blobSvc;
        _opt = opt.Value;
    }

    public async Task<CreateBrandingUploadResponse> CreateUploadAsync(
        Guid accountId,
        CreateBrandingUploadRequest req,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.ContentType))
            throw new ArgumentException("ContentType is required.");

        if (!AllowedContentTypes.Contains(req.ContentType))
            throw new ArgumentException($"Unsupported content type: {req.ContentType}");

        var container = _blobSvc.GetBlobContainerClient(_opt.Container);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

        var ext = GuessExtension(req.ContentType, req.FileName);
        var safeSlot = req.AssetType.ToString(); // LogoLight etc.

        // Stable path per account + slot, with a versioned filename for cache busting
        var version = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
        var blobName = $"accounts/{accountId}/branding/{safeSlot}/{safeSlot}-{version}{ext}";

        var blob = container.GetBlobClient(blobName);

        // Set content type on blob during upload (client must send x-ms-blob-content-type and Content-Type)
        // We only mint SAS; client will upload with headers.

        // Mint SAS for write (create + write)
        if (!blob.CanGenerateSasUri)
            throw new InvalidOperationException("Blob client cannot generate SAS. Check credentials.");

        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(_opt.SasMinutes);

        var sas = new BlobSasBuilder
        {
            BlobContainerName = container.Name,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = expiresOn
        };

        sas.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        var uploadUri = blob.GenerateSasUri(sas);

        return new CreateBrandingUploadResponse
        {
            
            UploadUrl = uploadUri.ToString(),
            BlobUrl = blob.Uri.ToString(),
            PendingBlobName = blobName,
            ExpiresUtc = expiresOn,
            AssetType = req.AssetType
        };
    }

    public async Task DeleteAsync(Guid accountId, BrandingAssetType assetType, CancellationToken ct)
    {
        // Optional: if you want delete to remove latest. That requires DB tracking blobName or list prefix.
        // Recommended: store the blobName in DB (not just URL) OR store both.
        throw new NotImplementedException("Implement once DB tracks blobName or you list by prefix.");
    }

    private static string GuessExtension(string contentType, string? fileName)
    {
        // prefer filename extension if provided
        var ext = Path.GetExtension(fileName ?? "");
        if (!string.IsNullOrWhiteSpace(ext) && ext.Length <= 6) return ext.ToLowerInvariant();

        return contentType.ToLowerInvariant() switch
        {
            "image/png" => ".png",
            "image/jpeg" => ".jpg",
            "image/webp" => ".webp",
            "image/svg+xml" => ".svg",
            _ => ""
        };
    }
}