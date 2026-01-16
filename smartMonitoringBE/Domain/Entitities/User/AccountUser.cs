namespace smartMonitoringBE.Domain.Entitities.User;

public enum AccountRole { Owner = 0, Admin = 1, Member = 2, Billing = 3, Viewer = 4 }

public class AccountUser
{
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public AccountRole Role { get; set; } = AccountRole.Member;
    public bool IsPrimary { get; set; } = true;

    public DateTimeOffset JoinedUtc { get; set; } = DateTimeOffset.UtcNow;
}