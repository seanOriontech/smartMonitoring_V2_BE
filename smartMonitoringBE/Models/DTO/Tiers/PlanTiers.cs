namespace smartMonitoringBE.Models.DTO.Tiers;

public sealed record TierCatalogDto(
    IReadOnlyList<PlanTierDto> Tiers
);

public sealed record PlanTierDto(
    string Code,                 // "starter"
    string Name,                 // "Starter"
    Guid PlanId,
    Guid PlanVersionId,
    int Version,
    bool IsTrial,
    int? TrialDays,
    string Currency,             // "ZAR"
    decimal Price,
    bool     ContactUs,
    string BillingInterval,      // "Monthly" | "Yearly" | "None"

    EntitlementsDto Entitlements
  

);

public sealed record EntitlementsDto(
    IReadOnlyDictionary<string, bool> Features,
    IReadOnlyDictionary<string, int> Limits
);