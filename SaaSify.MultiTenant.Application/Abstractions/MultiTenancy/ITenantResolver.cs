namespace SaaSify.MultiTenant.Application.Abstractions.MultiTenancy;

public interface ITenantResolver
{
    Task<TenantInfo?> ResolveAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    void Invalidate(Guid tenantId);
}
