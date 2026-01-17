using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Domain.Entitities.Tiers;

public enum BillingInterval { None = 0, Monthly = 1, Yearly = 2 }

public sealed class PlanVersion
{
    public Guid Id { get; set; }

    public Guid PlanId { get; set; }
    public Plan Plan { get; set; } = null!;

    public int Version { get; set; } // 1,2,3...

    public bool IsActive { get; set; } = true;

    // Entitlements for THIS version
    public string EntitlementsJson { get; set; } = "{}";

    // Pricing
    public string Currency { get; set; } = "ZAR";     // "ZAR", "USD"
    public decimal Price { get; set; } = 0m;          // 0 for free/trial if you want
    public BillingInterval BillingInterval { get; set; } = BillingInterval.Monthly;

    // Trial rules (only relevant if this plan/version supports trials)
    public bool IsTrial { get; set; } = false;
    public int? TrialDays { get; set; }               // e.g. 14, 30

    public bool ContactUs { get; set; } = false;   

    public DateTimeOffset EffectiveFromUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EffectiveToUtc { get; set; }

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}