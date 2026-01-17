namespace smartMonitoringBE.Domain.Entitities.Tiers;

public sealed class Plan
{
    public Guid Id { get; set; }

    public string Code { get; set; } = ""; // "starter", "growth", etc.
    public string Name { get; set; } = "";

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }

    public ICollection<PlanVersion> Versions { get; set; } = new List<PlanVersion>();
}