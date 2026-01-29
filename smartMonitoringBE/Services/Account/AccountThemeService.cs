using System.Text.Json;
using Microsoft.EntityFrameworkCore;

using smartMonitoringBE.Domain.Entitities.User;
using smartMonitoringBE.Infrastructure.Data;
using smartMonitoringBE.Models.DTO.Account;
using smartMonitoringBE.Models.Requests.Account;
using smartMonitoringBE.Services.Account;
using smartMonitoringBE.Services.Entitlements;

public sealed class AccountThemeService : IAccountThemeService
{
    private readonly SmartMonitoringDbContext _db;
    private readonly IEntitlementService _entitlements;

    private readonly Dictionary<string, string> _defaultLight;
    private readonly Dictionary<string, string> _defaultDark;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AccountThemeService(
        SmartMonitoringDbContext db,
        IEntitlementService entitlements,
        IConfiguration cfg)
    {
        _db = db;
        _entitlements = entitlements;

        _defaultLight = ParseOrEmpty(cfg["Theme:DefaultLightJson"]);
        _defaultDark  = ParseOrEmpty(cfg["Theme:DefaultDarkJson"]);
    }

    public async Task<AccountThemeDto> GetThemeAsync(Guid accountId, CancellationToken ct)
    {
        var customAllowed = await _entitlements.WhiteLabelEnabledAsync(accountId, ct);

        var acc = await _db.Accounts
            .AsNoTracking()
            .Where(a => a.Id == accountId)
            .Select(a => new
            {
                a.ThemePreference,
                a.ThemeLightJson,
                a.ThemeDarkJson
            })
            .SingleAsync(ct);

        // If not allowed → always return platform defaults.
        if (!customAllowed )
        {
            return new AccountThemeDto
            {
                ThemePreference = ThemePreference.Default,
                DefaultLight =  new Dictionary<string, string>(_defaultLight),
                DefaultDark  = new Dictionary<string, string>(_defaultDark),
                CustomThemeAllowed = false
            };
        }
        
       
        

        // Allowed → resolve theme from account (custom overrides) over defaults
        var lightCustom = ParseOrEmpty(acc.ThemeLightJson);
        var darkCustom  = ParseOrEmpty(acc.ThemeDarkJson);

        var resolvedLight = Merge(_defaultLight, lightCustom);
        var resolvedDark  = Merge(_defaultDark, darkCustom);

        return new AccountThemeDto
        {
            ThemePreference = acc.ThemePreference,
            DefaultLight =  new Dictionary<string, string>(_defaultLight),
            DefaultDark  = new Dictionary<string, string>(_defaultDark),
            CustomLight = resolvedLight,
            CustomDark = resolvedDark,
            CustomThemeAllowed = true
        };
    }

    public async Task<AccountThemeDto> PatchThemeAsync(Guid accountId, PatchAccountThemeRequest req, CancellationToken ct)
    {
        var customAllowed = await _entitlements.WhiteLabelEnabledAsync(accountId, ct);

        // If they are trying to use custom but aren’t allow´ed → reject.
        if (!customAllowed)
        {
            if (req.ThemePreference == ThemePreference.Custom || req.ThemeLight is not null || req.ThemeDark is not null)
                throw new InvalidOperationException("Your plan does not allow custom themes / white labelling.");
        }

        var acc = await _db.Accounts.SingleAsync(a => a.Id == accountId, ct);
        var now = DateTimeOffset.UtcNow;

        if (req.ThemePreference.HasValue)
        {
            // If custom not allowed, force Default
            acc.ThemePreference = customAllowed ? req.ThemePreference.Value : ThemePreference.Default;
        }

        if (customAllowed && req.ThemeLight is not null)
        {
            acc.ThemeLightJson = JsonSerializer.Serialize(req.ThemeLight, JsonOpts);
        }

        if (customAllowed && req.ThemeDark is not null)
        {
            acc.ThemeDarkJson = JsonSerializer.Serialize(req.ThemeDark, JsonOpts);
        }

        acc.ThemeUpdatedUtc = now;
        acc.UpdatedUtc = now;

        await _db.SaveChangesAsync(ct);

        // Return resolved DTO (same behaviour as GET)
        return await GetThemeAsync(accountId, ct);
    }

    // ---------------- helpers ----------------

    private static Dictionary<string, string> Merge(
        Dictionary<string, string> defaults,
        Dictionary<string, string> custom)
    {
        var merged = new Dictionary<string, string>(defaults);
        foreach (var kv in custom)
        {
            if (string.IsNullOrWhiteSpace(kv.Key)) continue;
            if (kv.Value is null) continue;
            merged[kv.Key] = kv.Value;
        }
        return merged;
    }

    private static Dictionary<string, string> ParseOrEmpty(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string>();

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOpts)
                   ?? new Dictionary<string, string>();
        }
        catch
        {
            // If bad JSON ever ends up in DB/config, fail safe to empty (defaults still apply)
            return new Dictionary<string, string>();
        }
    }
}