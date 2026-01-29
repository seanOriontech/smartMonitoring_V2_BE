using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.ENUMS;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO.Account;
using smartMonitoringBE.Models.DTO.Branding;
using smartMonitoringBE.Services.Entitlements;

namespace smartMonitoringBE.Services.Branding;



public sealed class BrandingService : IBrandingService
{
    private readonly SmartMonitoringDbContext _db;
    private readonly IEntitlementService _entitlements;

    // Set these from config
    private readonly string _defaultLogoLight;
    private readonly string _defaultLogoDark;
    private readonly string _defaultIconLight;
    private readonly string _defaultIconDark;

    public BrandingService(SmartMonitoringDbContext db, IEntitlementService entitlements, IConfiguration cfg)
    {
        _db = db;
        _entitlements = entitlements;

        _defaultLogoLight = cfg["Branding:DefaultLogoLight"]!;
        _defaultLogoDark  = cfg["Branding:DefaultLogoDark"]!;
        _defaultIconLight = cfg["Branding:DefaultIconLight"]!;
        _defaultIconDark  = cfg["Branding:DefaultIconDark"]!;
    }

    public async Task<BrandingDto> GetBrandingAsync(Guid accountId, CancellationToken ct)
    {
        var canUseCustom = await _entitlements.EnsureWhiteLabelEnabledAsync(accountId, ct);

        var acc = await _db.Accounts
            .AsNoTracking()
            .Where(a => a.Id == accountId)
            .Select(a => new
            {
                a.LogoLightUrl, a.LogoDarkUrl, a.IconLightUrl, a.IconDarkUrl
            })
            .SingleAsync(ct);

        if (!canUseCustom)
        {
            return new BrandingDto
            {
                LogoLightUrl = _defaultLogoLight,
                LogoDarkUrl = _defaultLogoDark,
                IconLightUrl = _defaultIconLight,
                IconDarkUrl = _defaultIconDark,
                UsingCustomBranding = false,
                CustomBrandingIsAvailable = false
            };
        }

        // Custom allowed – fallback to defaults if user hasn’t uploaded yet
        var hasAllAssets =
            !string.IsNullOrWhiteSpace(acc.LogoLightUrl) &&
            !string.IsNullOrWhiteSpace(acc.LogoDarkUrl) &&
            !string.IsNullOrWhiteSpace(acc.IconLightUrl) &&
            !string.IsNullOrWhiteSpace(acc.IconDarkUrl);

        return new BrandingDto
        {
            LogoLightUrl = acc.LogoLightUrl ?? _defaultLogoLight,
            LogoDarkUrl = acc.LogoDarkUrl ?? _defaultLogoDark,
            IconLightUrl = acc.IconLightUrl ?? _defaultIconLight,
            IconDarkUrl = acc.IconDarkUrl ?? _defaultIconDark,

            UsingCustomBranding = true,
            CustomBrandingIsAvailable = hasAllAssets
        };
    }
    
    public async Task<BrandingDto> SetBrandingAssetAsync(
        Guid accountId,
       PatchBrandingRequest req,
        CancellationToken ct)
    {
        var acc = await _db.Accounts
            .SingleOrDefaultAsync(a => a.Id == accountId, ct);

        if (acc is null)
            throw new KeyNotFoundException($"Account '{accountId}' not found.");

        var now = DateTimeOffset.UtcNow;

        // Centralised mutation logic (still nice)
        ApplyBrandingUpdate(acc, req, now);

        await _db.SaveChangesAsync(ct);

        return await  GetBrandingAsync(accountId, ct);
    }

    private static void ApplyBrandingUpdate(
        Domain.Entitities.User.Account acc,
        PatchBrandingRequest req,
        DateTimeOffset now)
    {
        switch (req.AssetType)
        {
            case BrandingAssetType.LogoLight:
                acc.LogoLightUrl = req.BlobUrl;
                acc.LogoLightContentType = req.ContentType;
                acc.LogoLightUpdatedUtc = now;
                acc.LogoLightBlobName = req.BlobName;
                break;

            case BrandingAssetType.LogoDark:
                acc.LogoDarkUrl = req.BlobUrl;
                acc.LogoDarkContentType = req.ContentType;
                acc.LogoDarkUpdatedUtc = now;
                acc.LogoDarkBlobName = req.BlobName;
                break;

            case BrandingAssetType.IconLight:
                acc.IconDarkUrl = req.BlobUrl;;
                acc.IconDarkContentType = req.ContentType;;
                acc.IconDarkUpdatedUtc = now;
                acc.IconDarkBlobName = req.BlobName;
                break;

            case BrandingAssetType.IconDark:
                acc.IconLightUrl = req.BlobUrl;;
                acc.IconLightContentType = req.ContentType;
                acc.IconLightUpdatedUtc = now;
                acc.IconLightBlobName = req.BlobName;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(req.AssetType), req.AssetType, null);
        }

        acc.BrandingUpdatedUtc = now;
        acc.UpdatedUtc = now;
    }
}