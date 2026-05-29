using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Factories;

public class TenantDbContextFactory
    : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(
        string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<TenantDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=TempTenantDb;Username=postgres;Password=root");

        return new TenantDbContext(
            optionsBuilder.Options);
    }
}