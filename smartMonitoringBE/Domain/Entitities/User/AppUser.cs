namespace smartMonitoringBE.Domain.Entitities.User;

public class AppUser
{
    public Guid Id { get; set; }

    // Entra External ID identity
    public string TenantId { get; set; } = "";    // tid claim
    public string ObjectId { get; set; } = "";    // oid claim (unique per tenant)

    public string? Email { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? DisplayName { get; set; }

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginUtc { get; set; }

    public ICollection<AccountUser> Accounts { get; set; } = new List<AccountUser>();
}