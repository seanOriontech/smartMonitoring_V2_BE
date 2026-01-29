namespace smartMonitoringBE.Services.Entitlements;

public interface IEntitlementService
{
    Task<Entitlements> GetEntitlementsAsync(Guid accountId, CancellationToken ct);

    Task<bool> HasFeatureAsync(Guid accountId,
        Func<FeaturesEntitlements, bool> selector,
        CancellationToken ct);

    Task<int> GetLimitAsync(Guid accountId,
        Func<LimitsEntitlements, int> selector,
        CancellationToken ct);

    Task<bool> WhiteLabelEnabledAsync(Guid accountId, CancellationToken ct);

    Task<bool>  EnsureWhiteLabelEnabledAsync(Guid accountId, CancellationToken ct);
}