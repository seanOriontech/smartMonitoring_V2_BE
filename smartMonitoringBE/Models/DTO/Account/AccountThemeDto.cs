using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Models.DTO.Account;

public sealed class AccountThemeDto
{
    public ThemePreference ThemePreference { get; set; } // Default / Custom
    public Dictionary<string,string>? CustomLight { get; set; }
    public Dictionary<string,string>? CustomDark { get; set; }
    
    public Dictionary<string,string>? DefaultLight { get; set; }
    public Dictionary<string,string>? DefaultDark { get; set; }

    // helpful for UI gating
    public bool CustomThemeAllowed { get; set; }
}