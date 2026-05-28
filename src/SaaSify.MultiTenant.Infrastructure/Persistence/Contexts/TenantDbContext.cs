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
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.EmailAddress)
                .IsRequired()
                .HasMaxLength(200);
        });
    }
}