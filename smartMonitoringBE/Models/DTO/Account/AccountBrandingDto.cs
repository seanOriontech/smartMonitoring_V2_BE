namespace smartMonitoringBE.Models.DTO.Account;

public sealed class AccountBrandingDto
{
    public Guid AccountId { get; set; }

    public string? LogoLightUrl { get; set; }
    public string? LogoDarkUrl { get; set; }
    public string? IconLightUrl { get; set; }
    public string? IconDarkUrl { get; set; }

    public DateTimeOffset? BrandingUpdatedUtc { get; set; }
}