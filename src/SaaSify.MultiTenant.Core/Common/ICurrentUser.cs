namespace SaaSify.MultiTenant.Core.Common;

public interface ICurrentUser
{
    Guid UserId { get; }

    string Email { get; }

    string Role { get; }

    Guid? TenantId { get; }
}