namespace smartMonitoringBE.Models.DTO.Options;

public sealed class AccountBrandingStorageOptions
{
    public string ConnectionString { get; set; } = "";
    public string Container { get; set; } = "account-branding";
    public int SasMinutes { get; set; } = 15;
}