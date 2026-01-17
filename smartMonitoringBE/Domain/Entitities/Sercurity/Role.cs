using smartMonitoringBE.Domain.Entitities.User;

namespace smartMonitoringBE.Domain.Entitities.Security;

public sealed class Role
{
    public Guid Id { get; set; }

    // NULL = global/system role template; otherwise role is owned by this account/tenant
    public Guid? AccountId { get; set; }

    // e.g. "owner", "admin", "operator", "viewer", "guest", or "custom-maintenance"
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    // true for built-in roles
    public bool IsSystem { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }

    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
    public ICollection<AccountUser> AccountUsers { get; set; } = new List<AccountUser>();
}