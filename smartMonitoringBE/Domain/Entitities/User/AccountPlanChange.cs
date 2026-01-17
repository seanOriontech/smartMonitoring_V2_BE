using smartMonitoringBE.Domain.Entitities.Tiers;

namespace smartMonitoringBE.Domain.Entitities.User;

public sealed class AccountPlanChange
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }
    public User.Account Account { get; set; } = null!;

    public Guid FromPlanVersionId { get; set; }
    public PlanVersion FromPlanVersion { get; set; } = null!;

    public Guid ToPlanVersionId { get; set; }
    public PlanVersion ToPlanVersion { get; set; } = null!;

    // Optional: keep your enum tier snapshot for easy reporting
    public User.PlanTier FromTier { get; set; }
    public User.PlanTier ToTier { get; set; }

    // Who changed it (user id from your identity system)
    public Guid? ChangedByUserId { get; set; } // null = system/automation
    public string? Reason { get; set; }        // e.g. "Upgrade via billing portal"

    public DateTimeOffset ChangedUtc { get; set; } = DateTimeOffset.UtcNow;
}