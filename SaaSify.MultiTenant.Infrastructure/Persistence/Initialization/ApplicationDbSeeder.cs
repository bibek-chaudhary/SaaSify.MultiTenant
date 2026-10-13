using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaaSify.MultiTenant.Core.Constants;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;
using SaaSify.MultiTenant.Infrastructure.Settings;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Initialization
{
    public static class ApplicationDbSeeder
    {
        public static async Task SeedAsync(MasterDbContext context, RoleManager<IdentityRole<Guid>> roleManager, UserManager<IdentityApplicationUser> userManager, SuperAdminSettings superAdmin)
        {
            await context.Database.MigrateAsync();

            var roles = new[]
            {
                Roles.SuperAdmin,
                Roles.Admin,
                Roles.Employee
            };

            foreach (var role in roles)
            {
                var exists = await roleManager.RoleExistsAsync(role);

                if (!exists)
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            if (string.IsNullOrWhiteSpace(superAdmin.Email))
            {
                throw new InvalidOperationException(
                    "SuperAdmin:Email is not configured.");
            }

            var userExists = await userManager.FindByEmailAsync(superAdmin.Email);

            if(userExists is null)
            {
                // Only needed for the first run; once the account exists the
                // password can be removed from configuration.
                if (string.IsNullOrWhiteSpace(superAdmin.Password))
                {
                    throw new InvalidOperationException(
                        "SuperAdmin:Password is not configured. " +
                        "Set it with `dotnet user-secrets` or the " +
                        "SuperAdmin__Password environment variable.");
                }

                var user = new IdentityApplicationUser {
                    Id = Guid.NewGuid(),
                    Email = superAdmin.Email,
                    UserName = superAdmin.Email,
                    EmailConfirmed = true,
                    TenantId = null
                };

                var result = await userManager.CreateAsync(user, superAdmin.Password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Failed to seed the SuperAdmin account: " +
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(user, Roles.SuperAdmin);
            }
        }
    }
}
