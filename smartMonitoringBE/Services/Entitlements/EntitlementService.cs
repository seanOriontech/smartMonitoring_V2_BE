using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Services.Entitlements;

public sealed class EntitlementService : IEntitlementService
{
    private readonly SmartMonitoringDbContext _db;
    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public EntitlementService(SmartMonitoringDbContext db) => _db = db;

    public async Task<Entitlements> GetEntitlementsAsync(Guid accountId, CancellationToken ct)
    {
        // Adjust field names to match your schema
        var json = await _db.Accounts
            .Where(a => a.Id == accountId)
            .Select(a => a.PlanVersion!.EntitlementsJson) // <-- update if different
            .SingleAsync(ct);

        if (string.IsNullOrWhiteSpace(json))
            return new Entitlements();

        try
        {
            return JsonSerializer.Deserialize<Entitlements>(json, _jsonOpts) ?? new Entitlements();
        }
        catch
        {
            // If malformed JSON ever slips through, fail safe
            return new Entitlements();
        }
    }

    public async Task<bool> HasFeatureAsync(
        Guid accountId,
        Func<FeaturesEntitlements, bool> selector,
        CancellationToken ct)
    {
        var e = await GetEntitlementsAsync(accountId, ct);
        return selector(e.Features);
    }

    public async Task<int> GetLimitAsync(
        Guid accountId,
        Func<LimitsEntitlements, int> selector,
        CancellationToken ct)
    {
        var e = await GetEntitlementsAsync(accountId, ct);
        return selector(e.Limits);
    }

    // ---- Convenience shortcuts ----

    public Task<bool> WhiteLabelEnabledAsync(Guid accountId, CancellationToken ct)
        => HasFeatureAsync(accountId, f => f.WhiteLabel, ct);

    public Task<int> RetentionDaysAsync(Guid accountId, CancellationToken ct)
        => GetLimitAsync(accountId, l => l.RetentionDays, ct);
    
    public async Task<bool> EnsureWhiteLabelEnabledAsync(Guid accountId, CancellationToken ct)
    {
       return await WhiteLabelEnabledAsync(accountId, ct);
     
    }
}