namespace smartMonitoringBE.Security;

public interface IAuthorizationCache
{
    Task<AuthzContext?> GetOrCreateAsync(Guid userId, Guid accountId, Func<Task<AuthzContext?>> factory);
    void Invalidate(Guid userId, Guid accountId);
}