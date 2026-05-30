using SaaSify.MultiTenant.Application.Common.Interfaces;
using SaaSify.MultiTenant.Core.Common;

namespace SaaSify.MultiTenant.Infrastructure.MultiTenancy;

public class TenantProvider : ITenantProvider
{
    private TenantInfo? _tenant;

    public TenantInfo? GetCurrentTenant()
    {
        return _tenant;
    }

    public void SetTenant(TenantInfo tenant)
    {
        _tenant = tenant;
    }
}