namespace smartMonitoringBE.Security;

public interface IAuthzContextAccessor
{
    Task<AuthzContext> GetAsync(Guid accountId, CancellationToken ct);
}