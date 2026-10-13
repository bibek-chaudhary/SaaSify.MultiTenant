namespace SaaSify.MultiTenant.Application.Abstractions.MultiTenancy;

public interface ITenantProvider
{
    TenantInfo? GetCurrentTenant();

    void SetTenant(TenantInfo tenant);
}
