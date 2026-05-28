using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Identity.Core;
using SaaSify.MultiTenant.Application.Interfaces;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using SaaSify.MultiTenant.Infrastructure.Identity.Services;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MasterDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("MasterConnection"));
        });

        services
            .AddIdentity<IdentityApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;

                options.Password.RequireUppercase = true;

                options.Password.RequireLowercase = true;

                options.Password.RequireDigit = true;

                options.Password.RequireNonAlphanumeric = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<MasterDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}