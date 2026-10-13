using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Factories;

public class TenantDbContextFactory
    : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(
        string[] args)
    {
        // Tenant databases are created at runtime; design time only needs
        // a server to point at, so reuse the master server credentials.
        var builder =
            new NpgsqlConnectionStringBuilder(
                DesignTimeConfiguration.GetMasterConnection())
            {
                Database = "TempTenantDb"
            };

        var optionsBuilder =
            new DbContextOptionsBuilder<TenantDbContext>();

        optionsBuilder.UseNpgsql(
            builder.ConnectionString);

        return new TenantDbContext(
            optionsBuilder.Options);
    }
}
