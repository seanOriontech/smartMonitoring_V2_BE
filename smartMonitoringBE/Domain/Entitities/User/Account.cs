namespace smartMonitoringBE.Domain.Entitities.User;

public enum AccountType { None = 0, Individual = 1, Business = 2 }
public enum PlanTier { None = 0 , Trail = 1 , Starter = 2, Growth = 3, Business = 4, Enterprise = 5 }

public class Account
{
    public Guid Id { get; set; }

    public AccountType Type { get; set; }
    public PlanTier Tier { get; set; } = PlanTier.Starter;

    public string? Industry { get; set; } 
    public string Name { get; set; } = ""; // Person name or Business name

    // Common
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }

    // Business fields (nullable for Individual)
    public string? VatNumber { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    // Logo
    public string? LogoUrl { get; set; }     // if stored in blob storage / S3
    public byte[]? LogoBytes { get; set; }   // if stored in DB (optional v1)
    public string? LogoContentType { get; set; }

    // Address (optional v1)
    public Address? Address { get; set; }

    public ICollection<AccountUser> Users { get; set; } = new List<AccountUser>();
}