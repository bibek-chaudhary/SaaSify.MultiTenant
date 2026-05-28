using Microsoft.AspNetCore.Identity;
using SaaSify.MultiTenant.Application.Interfaces;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;

namespace SaaSify.MultiTenant.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityApplicationUser> _userManager;

    public IdentityService(
        UserManager<IdentityApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Success, Guid UserId, string Email, string Role, Guid TenantId)>
        ValidateUserAsync(string email, string password)
    {
        var user =
            await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return default;
        }

        var validPassword =
            await _userManager.CheckPasswordAsync(
                user,
                password);

        if (!validPassword)
        {
            return default;
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault() ?? "User";

        return (
            true,
            user.Id,
            user.Email!,
            role,
            user.TenantId ?? Guid.Empty
        );
    }
}