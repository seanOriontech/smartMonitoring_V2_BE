using smartMonitoringBE.Domain.Entitities.Structure;
using smartMonitoringBE.Domain.Entitities.Tiers;
using smartMonitoringBE.Domain.Entitities.User.Group;

namespace smartMonitoringBE.Domain.Entitities.User;
public enum ThemePreference { Default = 0, Custom = 1 }

public enum AccountType { None = 0, Individual = 1, Business = 2 }
public enum PlanTier { None = 0 , Trail = 1 , Starter = 2, Growth = 3, Business = 4, Enterprise = 5 }

public class Account
{
    public Guid Id { get; set; }

    public AccountType Type { get; set; }
    public PlanTier Tier { get; set; } = PlanTier.Starter;

    public Guid PlanVersionId { get; set; }
    public PlanVersion? PlanVersion { get; set; } = null!;

    public DateTimeOffset? PlanStartedDateTime { get; set; }

    public string? Industry { get; set; }
    public string Name { get; set; } = "";

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }

    public string? VatNumber { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    // --- Branding assets (Blob Storage URLs) ---

    // Logos
// Logos
    public string? LogoLightBlobName { get; set; }   // e.g. branding/{accountId}/logo-light.png
    public string? LogoLightUrl { get; set; }
    public string? LogoLightContentType { get; set; }
    public DateTimeOffset? LogoLightUpdatedUtc { get; set; }

    public string? LogoDarkBlobName { get; set; }
    public string? LogoDarkUrl { get; set; }
    public string? LogoDarkContentType { get; set; }
    public DateTimeOffset? LogoDarkUpdatedUtc { get; set; }

    
    public ThemePreference ThemePreference { get; set; } = ThemePreference.Default;

    public DateTimeOffset ThemeUpdatedUtc { get; set; } 
    // JSON stored as string (or owned type if you prefer)
    public string? ThemeLightJson { get; set; }   // e.g. {"--bg":"#fff", "--accent":"#..."}
    public string? ThemeDarkJson { get; set; }
    
// Icons
    public string? IconLightBlobName { get; set; }
    public string? IconLightUrl { get; set; }
    public string? IconLightContentType { get; set; }
    public DateTimeOffset? IconLightUpdatedUtc { get; set; }

    public string? IconDarkBlobName { get; set; }
    public string? IconDarkUrl { get; set; }
    public string? IconDarkContentType { get; set; }
    public DateTimeOffset? IconDarkUpdatedUtc { get; set; }

    // Optional: if you want a single "branding changed" cache buster
    public DateTimeOffset? BrandingUpdatedUtc { get; set; }

    // Address
    public Address? Address { get; set; }

    public ICollection<AccountUser> Users { get; set; } = new List<AccountUser>();
    public ICollection<AccountPlanChange> PlanChanges { get; set; } = new List<AccountPlanChange>();
    public ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
    
    public ICollection<AccountGroup> Groups { get; set; } = new List<AccountGroup>();
}