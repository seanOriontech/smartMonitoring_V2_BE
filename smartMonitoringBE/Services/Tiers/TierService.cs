using System.Text.Json;
using Microsoft.EntityFrameworkCore;

using smartMonitoringBE.Domain.Entitities.Tiers;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO.Tiers;
using smartMonitoringBE.Services.Tiers;

public sealed class TiersService : ITiersService
{
    private readonly SmartMonitoringDbContext _db;

    public TiersService(SmartMonitoringDbContext db) => _db = db;

    public async Task<TierCatalogDto> GetLatestAsync(CancellationToken ct)
    {
        try
        {
  var now = DateTimeOffset.UtcNow;

        // Get latest active version per plan
        var rows = await _db.PlanVersions
            .AsNoTracking()
            .Where(v =>
                v.IsActive &&
                v.Plan.IsActive &&
                v.EffectiveFromUtc <= now &&
                (v.EffectiveToUtc == null || v.EffectiveToUtc > now))
            .GroupBy(v => v.PlanId)
            .Select(g => g
                .OrderByDescending(x => x.Version)
                .Select(x => new
                {
                    PlanId = x.PlanId,
                    PlanCode = x.Plan.Code,
                    PlanName = x.Plan.Name,
                    PlanVersionId = x.Id,
                    x.Version,
                    x.IsTrial,
                    x.TrialDays,
                    x.Currency,
                    x.Price,
                    x.ContactUs,
                    x.BillingInterval,
                    x.EntitlementsJson
                })
                .First())
            .ToListAsync(ct);

        // Optional: stable ordering for the FE
        var order = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["trial"] = 0,
            ["starter"] = 1,
            ["growth"] = 2,
            ["business"] = 3,
            ["enterprise"] = 4
        };

        var tiers = rows
            .OrderBy(r => order.TryGetValue(r.PlanCode, out var i) ? i : 999)
            .Select(r => new PlanTierDto(
                Code: r.PlanCode,
                Name: r.PlanName,
                PlanId: r.PlanId,
                PlanVersionId: r.PlanVersionId,
                Version: r.Version,
                IsTrial: r.IsTrial,
                TrialDays: r.TrialDays,
                Currency: r.Currency,
                Price: r.Price,
             ContactUs: r.ContactUs,
                BillingInterval: r.BillingInterval.ToString(),
                Entitlements: ParseEntitlements(r.EntitlementsJson)
                
            ))
            .ToList();

        return new TierCatalogDto(tiers);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
      
    }

    private static EntitlementsDto ParseEntitlements(string json)
    {
        using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
        var root = doc.RootElement;

        var features = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        if (root.TryGetProperty("features", out var f) && f.ValueKind == JsonValueKind.Object)
        {
            foreach (var p in f.EnumerateObject())
                features[p.Name] = p.Value.ValueKind == JsonValueKind.True;
        }

        var limits = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (root.TryGetProperty("limits", out var l) && l.ValueKind == JsonValueKind.Object)
        {
            foreach (var p in l.EnumerateObject())
            {
                if (p.Value.ValueKind == JsonValueKind.Number && p.Value.TryGetInt32(out var v))
                    limits[p.Name] = v;
            }
        }

        return new EntitlementsDto(features, limits);
    }
}

