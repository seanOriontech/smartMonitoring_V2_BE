namespace smartMonitoringBE.Services.Entitlements;

public sealed class Entitlements
{
    public FeaturesEntitlements Features { get; set; } = new();
    public LimitsEntitlements Limits { get; set; } = new();
}

public sealed class FeaturesEntitlements
{
    public bool Alerts { get; set; }
    public bool Dashboards { get; set; }
    public bool ApiAccess { get; set; }
    public bool WhiteLabel { get; set; }
}

public sealed class LimitsEntitlements
{
    public int MaxDevices { get; set; }
    public int MaxUsers { get; set; }
    public int MaxDashboards { get; set; }
    public int MaxAlertRules { get; set; }
    public int RetentionDays { get; set; }
}