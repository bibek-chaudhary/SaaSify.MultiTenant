namespace SaaSify.MultiTenant.Core.Common;

public interface ITenantProvider
{
    Guid? GetTenantId();
}