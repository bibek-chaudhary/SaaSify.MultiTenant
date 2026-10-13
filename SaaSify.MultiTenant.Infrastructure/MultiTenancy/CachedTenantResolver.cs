using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SaaSify.MultiTenant.Application.Abstractions.MultiTenancy;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure.MultiTenancy;

public class CachedTenantResolver : ITenantResolver
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _cache;

    private readonly MasterDbContext _masterDbContext;

    public CachedTenantResolver(
        IMemoryCache cache,
        MasterDbContext masterDbContext)
    {
        _cache = cache;
        _masterDbContext = masterDbContext;
    }

    public async Task<TenantInfo?> ResolveAsync(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(CacheKey(tenantId), out TenantInfo? cached))
        {
            return cached;
        }

        var tenant =
            await _masterDbContext.Tenants
                .AsNoTracking()
                .Where(x => x.Id == tenantId)
                .Select(x => new TenantInfo
                {
                    TenantId = x.Id,
                    Identifier = x.TenantId,
                    ConnectionString = x.DbConnStr
                })
                .FirstOrDefaultAsync(cancellationToken);

        // Misses aren't cached, so a missing tenant is always re-checked.
        if (tenant is not null)
        {
            _cache.Set(CacheKey(tenantId), tenant, CacheDuration);
        }

        return tenant;
    }

    public void Invalidate(Guid tenantId)
    {
        _cache.Remove(CacheKey(tenantId));
    }

    private static string CacheKey(Guid tenantId) => $"tenant:{tenantId}";
}
