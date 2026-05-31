namespace SaaSify.MultiTenant.Application.Abstractions.Authentication;
public interface ICurrentUserService
{
    Guid UserId { get; }

    string Email { get; }

    string Role { get; }

    Guid? TenantId { get; }

    bool IsAuthenticated { get; }
}
