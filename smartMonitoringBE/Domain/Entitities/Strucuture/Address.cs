

namespace smartMonitoringBE.Domain.Entitities.Structure;

public sealed class Address
{
    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "ZA";
}