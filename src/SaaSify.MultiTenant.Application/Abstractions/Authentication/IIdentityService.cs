using SaaSify.MultiTenant.Application.Features.Auth.DTOs;

namespace SaaSify.MultiTenant.Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<(bool Success, Guid UserId, string Email, string Role, Guid? TenantId)>
        ValidateUserAsync(string email, string password);

    Task CreateTenantAdminAsync(
        string email,
        Guid tenantId,
        string password);

    Task<Guid> RegisterUserAsync(
        string email,
        string password,
        string role,
        Guid tenantId);
}