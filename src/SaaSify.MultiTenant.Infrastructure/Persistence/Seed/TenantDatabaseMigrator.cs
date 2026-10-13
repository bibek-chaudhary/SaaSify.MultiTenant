using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Seed
{
    public static class TenantDatabaseMigrator
    {
        // Tenant databases are only migrated on creation, so existing ones are
        // brought up to date here and any employees missing a UserId are linked
        // to their Identity user by email.
        public static async Task MigrateAllAsync(
            MasterDbContext masterContext,
            UserManager<IdentityApplicationUser> userManager,
            ILogger logger)
        {
            var tenants = await masterContext.Tenants
                .AsNoTracking()
                .ToListAsync();

            foreach (var tenant in tenants)
            {
                try
                {
                    var options = new DbContextOptionsBuilder<TenantDbContext>()
                        .UseNpgsql(tenant.DbConnStr)
                        .Options;

                    await using var tenantContext = new TenantDbContext(options);

                    await tenantContext.Database.MigrateAsync();

                    var unlinkedEmployees = await tenantContext.Employees
                        .Where(x => x.UserId == Guid.Empty)
                        .ToListAsync();

                    foreach (var employee in unlinkedEmployees)
                    {
                        var user =
                            await userManager.FindByEmailAsync(employee.EmailAddress);

                        if (user is null || user.TenantId != tenant.Id)
                        {
                            logger.LogWarning(
                                "No Identity user found for employee {EmployeeId} in tenant {TenantId}.",
                                employee.Id,
                                tenant.TenantId);

                            continue;
                        }

                        employee.UserId = user.Id;
                    }

                    await tenantContext.SaveChangesAsync();
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "Failed to migrate database for tenant {TenantId}.",
                        tenant.TenantId);
                }
            }
        }
    }
}
