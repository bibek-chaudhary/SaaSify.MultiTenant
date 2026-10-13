using SaaSify.MultiTenant.Application.Common;

namespace SaaSify.MultiTenant.Application.Common.Interfaces;

public interface ITenantResolver
{
    Task<TenantInfo?> ResolveAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    void Invalidate(Guid tenantId);
}
