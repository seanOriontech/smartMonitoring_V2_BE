namespace smartMonitoringBE.Domain.Entitities.Security;

public sealed class Permission
{
    public Guid Id { get; set; }

    // e.g. "devices.read", "alerts.acknowledge"
    public string Code { get; set; } = "";

    // e.g. "Devices", "Alerts", "Users"
    public string Group { get; set; } = "";

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }

    public ICollection<RolePermission> Roles { get; set; } = new List<RolePermission>();
}