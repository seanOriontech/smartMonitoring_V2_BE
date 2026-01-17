namespace smartMonitoringBE.Domain.Entitities.Structure;

public sealed class Device
{
    public Guid Id { get; set; }


    public string Name { get; set; } = "";
    
    
    // ✅ NEW: device-level location (overrides/in addition to POI)
    public double? Lat { get; set; }
    public double? Lng { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }
    public DateTimeOffset? ArchivedUtc { get; set; }

    // for later
    public string? MetadataJson { get; set; }
}