namespace SaaSify.MultiTenant.Application.Interfaces;

public interface ITenantDatabaseService
{
    Task<string> CreateTenantDatabaseAsync(string tenantIdentifier);
}