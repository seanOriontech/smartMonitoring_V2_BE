namespace smartMonitoringBE.Domain.Entitities.User.Group;

public sealed class AccountUserGroup
{
    public Guid AccountId { get; set; }
    public Guid UserId { get; set; }

    public Guid GroupId { get; set; }

    public AccountUser AccountUser { get; set; } = null!;
    public AccountGroup Group { get; set; } = null!;

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
}