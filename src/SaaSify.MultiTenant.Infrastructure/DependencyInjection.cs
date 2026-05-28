using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Identity.Core;
using Microsoft.IdentityModel.Tokens;
using SaaSify.MultiTenant.Application.Interfaces;
using SaaSify.MultiTenant.Infrastructure.Identity;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using SaaSify.MultiTenant.Infrastructure.Identity.Services;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;
using System.Text;

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

        services.Configure<JwtSettings>( configuration.GetSection("Jwt"));

        var jwtSettings =
                    configuration.GetSection("Jwt")
                        .Get<JwtSettings>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,

                    ValidateAudience = true,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings!.Issuer,

                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdminOnly", policy =>
            {
                policy.RequireRole("SuperAdmin");
            });

            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireRole("Admin");
            });

            options.AddPolicy("EmployeeOnly", policy =>
            {
                policy.RequireRole("Employee");
            });
        });

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}