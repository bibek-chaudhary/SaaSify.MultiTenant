using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using System.Security.Claims;

namespace SaaSify.MultiTenant.Infrastructure.Authentication;

// Rejects JWTs whose security stamp no longer matches the user's current one,
// so deleting a user or changing their role/email revokes existing tokens.
public class JwtSecurityStampValidator
{
    public const string ClaimType = "sstamp";

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _cache;

    private readonly UserManager<IdentityApplicationUser> _userManager;

    public JwtSecurityStampValidator(
        IMemoryCache cache,
        UserManager<IdentityApplicationUser> userManager)
    {
        _cache = cache;
        _userManager = userManager;
    }

    public async Task<bool> ValidateAsync(ClaimsPrincipal principal)
    {
        var tokenStamp =
            principal.FindFirstValue(ClaimType);

        if (string.IsNullOrEmpty(tokenStamp)
            || !Guid.TryParse(
                principal.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return false;
        }

        var currentStamp =
            await GetCurrentStampAsync(userId);

        return currentStamp is not null
            && string.Equals(currentStamp, tokenStamp, StringComparison.Ordinal);
    }

    public void Invalidate(Guid userId)
    {
        _cache.Remove(CacheKey(userId));
    }

    private async Task<string?> GetCurrentStampAsync(Guid userId)
    {
        if (_cache.TryGetValue(CacheKey(userId), out string? cached))
        {
            return cached;
        }

        var user =
            await _userManager.FindByIdAsync(userId.ToString());

        var stamp = user?.SecurityStamp;

        // Deleted users aren't cached, so they're always re-checked.
        if (stamp is not null)
        {
            _cache.Set(CacheKey(userId), stamp, CacheDuration);
        }

        return stamp;
    }

    private static string CacheKey(Guid userId) => $"sstamp:{userId}";
}
