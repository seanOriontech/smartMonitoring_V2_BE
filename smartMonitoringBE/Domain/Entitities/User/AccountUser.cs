using smartMonitoringBE.Domain.Entitities.Security;
using smartMonitoringBE.Domain.Entitities.User.Group;


namespace smartMonitoringBE.Domain.Entitities.User;



public class AccountUser
{
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public bool IsPrimary { get; set; } = true;
    
    public bool IsDefault { get; set; } = false;

    public DateTimeOffset JoinedUtc { get; set; } = DateTimeOffset.UtcNow;
    
    public ICollection<AccountUserScope> Scopes { get; set; } = new List<AccountUserScope>();
    
    public ICollection<AccountUserGroup> Groups { get; set; } = new List<AccountUserGroup>();
}