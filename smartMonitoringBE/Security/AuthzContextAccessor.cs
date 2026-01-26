namespace smartMonitoringBE.Security;


public sealed class AuthzContextAccessor : IAuthzContextAccessor
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationContextLoader _loader;
    private readonly IAuthorizationCache _cache;

    public AuthzContextAccessor(
        ICurrentUser currentUser,
        IAuthorizationContextLoader loader,
        IAuthorizationCache cache)
    {
        _currentUser = currentUser;
        _loader = loader;
        _cache = cache;
    }

    public async Task<AuthzContext> GetAsync(Guid accountId, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var ctx = await _cache.GetOrCreateAsync(userId, accountId, () => _loader.Load(userId, accountId, ct));

        if (ctx is null)
            throw new UnauthorizedAccessException("User is not a member of this account.");

        return ctx;
    }
}