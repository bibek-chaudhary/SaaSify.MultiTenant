using Microsoft.EntityFrameworkCore;
using SaaSify.MultiTenant.Core.Entities;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

public class TenantDbContext : DbContext
{
    public TenantDbContext(
        DbContextOptions<TenantDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(
     ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TenantDbContext).Assembly);
    }
}