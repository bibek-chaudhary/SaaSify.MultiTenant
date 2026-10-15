using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Core.Constants;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;

namespace SaaSify.MultiTenant.Infrastructure.Authentication;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityApplicationUser> _userManager;

    private readonly SignInManager<IdentityApplicationUser> _signInManager;

    private readonly JwtSecurityStampValidator _securityStampValidator;

    public IdentityService(
        UserManager<IdentityApplicationUser> userManager,
        SignInManager<IdentityApplicationUser> signInManager,
        JwtSecurityStampValidator securityStampValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _securityStampValidator = securityStampValidator;
    }

    public async Task<(bool Success, Guid UserId, string Email, string Role, Guid? TenantId)>
        ValidateUserAsync(string email, string password)
    {
        var user =
            await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return default;
        }

        // Counts failed attempts and enforces Identity's lockout settings,
        // unlike UserManager.CheckPasswordAsync.
        var result =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            throw new UnauthorizedException(
                "Account is temporarily locked due to too many failed login attempts. Try again later.");
        }

        if (!result.Succeeded)
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
            user.TenantId
        );
    }

    public Task<Guid> CreateTenantAdminAsync(
        string email,
        Guid tenantId,
        string password)
    {
        return RegisterUserAsync(
            email,
            password,
            Roles.Admin,
            tenantId);
    }

    public async Task<Guid> RegisterUserAsync(
        string email,
        string password,
        string role,
    Guid tenantId)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            throw new ConflictException(
                "User already exists.");
        }

        var user =
            new IdentityApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                TenantId = tenantId,
                EmailConfirmed = true
            };

        var result =
            await _userManager.CreateAsync(
                user,
                password);

        if (!result.Succeeded)
        {
            throw new ConflictException(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            throw new InvalidOperationException(
                $"Failed to assign role '{role}': " +
                string.Join(", ",
                    roleResult.Errors.Select(x => x.Description)));
        }

        return user.Id;
    }

    public async Task<Guid?> FindUserIdByEmailAsync(string email)
    {
        var user =
            await _userManager.FindByEmailAsync(email);

        return user?.Id;
    }

    public async Task UpdateEmailAsync(Guid userId, string newEmail)
    {
        var user =
            await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            throw new NotFoundException(
                $"Identity user '{userId}' not found.");
        }

        if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var existingUser =
            await _userManager.FindByEmailAsync(newEmail);

        if (existingUser is not null)
        {
            throw new ConflictException(
                "User already exists.");
        }

        user.Email = newEmail;
        user.UserName = newEmail;

        var result =
            await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new ConflictException(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }

        // Existing tokens carry the old email, so revoke them.
        await _userManager.UpdateSecurityStampAsync(user);

        _securityStampValidator.Invalidate(user.Id);
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user =
            await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            throw new NotFoundException(
                $"Identity user '{userId}' not found.");
        }

        await _userManager.DeleteAsync(user);

        _securityStampValidator.Invalidate(user.Id);
    }

    public async Task DeleteUsersByTenantAsync(Guid tenantId)
    {
        var users =
            await _userManager.Users
                .Where(x => x.TenantId == tenantId)
                .ToListAsync();

        foreach (var user in users)
        {
            var result =
                await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to delete user '{user.Id}': " +
                    string.Join(", ",
                        result.Errors.Select(x => x.Description)));
            }

            _securityStampValidator.Invalidate(user.Id);
        }
    }
}