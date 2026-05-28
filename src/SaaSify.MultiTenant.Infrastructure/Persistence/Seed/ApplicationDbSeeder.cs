using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaaSify.MultiTenant.Core.Constants;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Seed
{
    public static class ApplicationDbSeeder
    {
        public static async Task SeedAsync(MasterDbContext context, RoleManager<IdentityRole<Guid>> roleManager, UserManager<IdentityApplicationUser> userManager)
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

            var email = "assessment@yopmail.com";

            var userExists = await userManager.FindByEmailAsync(email);

            if(userExists is null)
            {
                var user = new IdentityApplicationUser {
                    Id = Guid.NewGuid(),
                    Email = email,
                    UserName = email,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(),
                    EmailConfirmed = true,
                    TenantId = null
                };

                var result = await userManager.CreateAsync(user, "Tester@123");

                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.SuperAdmin);
                }
            }
        }
    }
}
