using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Factories;

public class MasterDbContextFactory
    : IDesignTimeDbContextFactory<MasterDbContext>
{
    public MasterDbContext CreateDbContext(
        string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<MasterDbContext>();

        optionsBuilder.UseNpgsql(
            DesignTimeConfiguration.GetMasterConnection());

        return new MasterDbContext(
            optionsBuilder.Options);
    }
}
