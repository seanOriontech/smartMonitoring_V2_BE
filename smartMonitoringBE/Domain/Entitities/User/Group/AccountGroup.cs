using smartMonitoringBE.Domain.Entitities.Security;

namespace smartMonitoringBE.Domain.Entitities.User.Group;

public sealed class AccountGroup
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public string Name { get; set; } = "";
    public string Code { get; set; } = ""; // optional, nice for built-ins like "operators-plant-a"

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }

    public ICollection<AccountGroupScope> Scopes { get; set; } = new List<AccountGroupScope>();
    public ICollection<AccountUserGroup> Users { get; set; } = new List<AccountUserGroup>();
}