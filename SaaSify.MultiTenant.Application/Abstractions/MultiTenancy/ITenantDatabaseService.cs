namespace SaaSify.MultiTenant.Application.Abstractions.MultiTenancy;

public interface ITenantDatabaseService
{
    Task<string> CreateTenantDatabaseAsync(string tenantIdentifier);

    Task DeleteTenantDatabaseAsync(string tenantIdentifier);
}