namespace SaaSify.MultiTenant.Application.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(Guid userId, string email, string role, Guid? tenantId);
}