using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SaaSify.MultiTenant.Core.Entities;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Contexts
{
    public class MasterDbContext
            : IdentityDbContext<IdentityApplicationUser, IdentityRole<Guid>, Guid>
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Tenant>(entity =>
                {
                    entity.HasKey(t => t.Id);
                    entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                    entity.Property(t => t.EmailAddress).IsRequired().HasMaxLength(255);
                    entity.Property(t => t.TenantId).IsRequired().HasMaxLength(4);
                    entity.Property(t => t.DbConnStr).IsRequired();
                    entity.HasIndex(t => t.TenantId).IsUnique();
                });
        }
    }
}
