using SaaSify.MultiTenant.Application.Features.Auth.DTOs;

namespace SaaSify.MultiTenant.Application.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, Guid UserId, string Email, string Role, Guid TenantId)>
        ValidateUserAsync(string email, string password);
}