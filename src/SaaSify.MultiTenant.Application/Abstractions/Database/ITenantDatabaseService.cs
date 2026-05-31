namespace SaaSify.MultiTenant.Application.Abstractions.Database;

public interface ITenantDatabaseService
{
    Task<string> CreateTenantDatabaseAsync(string tenantIdentifier);

    Task DeleteTenantDatabaseAsync(string tenantIdentifier);
}