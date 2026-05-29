using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Factories;

public class MasterDbContextFactory
    : IDesignTimeDbContextFactory<MasterDbContext>
{
    public MasterDbContext CreateDbContext(
        string[] args)
    {
        var configuration =
            new ConfigurationBuilder()
                .SetBasePath(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "../SaaSify.MultiTenant.Api"))
                .AddJsonFile(
                    "appsettings.json")
                .Build();

        var connectionString =
            configuration.GetConnectionString(
                "MasterConnection");

        var optionsBuilder =
            new DbContextOptionsBuilder<MasterDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new MasterDbContext(
            optionsBuilder.Options);
    }
}