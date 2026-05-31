using SaaSify.MultiTenant.Application.Common;

namespace SaaSify.MultiTenant.Application.Common.Interfaces;

public interface ITenantProvider
{
    TenantInfo? GetCurrentTenant();

    void SetTenant(TenantInfo tenant);
}
