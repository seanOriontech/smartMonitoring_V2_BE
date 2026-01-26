using Microsoft.Extensions.Caching.Memory;

namespace smartMonitoringBE.Security;

public sealed class MemoryAuthorizationCache : IAuthorizationCache
{
    private readonly IMemoryCache _cache;

    public MemoryAuthorizationCache(IMemoryCache cache) => _cache = cache;

    private static string Key(Guid userId, Guid accountId) => $"authz:{accountId}:{userId}";

    public Task<AuthzContext?> GetOrCreateAsync(Guid userId, Guid accountId, Func<Task<AuthzContext?>> factory)
    {
        return _cache.GetOrCreateAsync(Key(userId, accountId), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3); // basic
            entry.SlidingExpiration = TimeSpan.FromMinutes(1);
            return await factory();
        });
    }

    public void Invalidate(Guid userId, Guid accountId)
        => _cache.Remove(Key(userId, accountId));
}