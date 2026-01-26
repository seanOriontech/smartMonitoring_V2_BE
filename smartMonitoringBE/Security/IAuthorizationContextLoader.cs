namespace smartMonitoringBE.Security;

public interface IAuthorizationContextLoader
{
    Task<AuthzContext?> Load(Guid userId, Guid accountId, CancellationToken ct);
}