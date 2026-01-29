using System.Text.Json.Serialization;
using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Models.Requests.Account;

public sealed class PatchAccountThemeRequest
{
    [JsonPropertyName("themePreference")]
    public ThemePreference? ThemePreference { get; set; }
    [JsonPropertyName("themeLight")]
    public Dictionary<string,string>? ThemeLight { get; set; }
    [JsonPropertyName("themeDark")]
    public Dictionary<string,string>? ThemeDark { get; set; }
}