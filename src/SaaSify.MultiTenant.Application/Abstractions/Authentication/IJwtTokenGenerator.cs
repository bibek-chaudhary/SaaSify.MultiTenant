namespace SaaSify.MultiTenant.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(Guid userId, string email, string role, Guid? tenantId);
}