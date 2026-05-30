using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Database;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Common.Interfaces;
using SaaSify.MultiTenant.Infrastructure.Authentication;
using SaaSify.MultiTenant.Infrastructure.Configurations;
using SaaSify.MultiTenant.Infrastructure.Database;
using SaaSify.MultiTenant.Infrastructure.Identity;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using SaaSify.MultiTenant.Infrastructure.MultiTenancy;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;
using SaaSify.MultiTenant.Infrastructure.Persistence.Repositories;
using SaaSify.MultiTenant.Shared.Responses;
using System.Text;

namespace SaaSify.MultiTenant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));

        services.AddDbContext<MasterDbContext>(options =>
        {
            options.UseNpgsql(configuration["DatabaseSettings:MasterConnection"]);
        });

        services.AddDbContext<TenantDbContext>(
            (serviceProvider, options) =>
            {
                var tenantProvider =
                    serviceProvider
                        .GetRequiredService<ITenantProvider>();

                var tenant =
                    tenantProvider.GetCurrentTenant();

                if (tenant is null)
                {
                    return;
                }

                options.UseNpgsql(
                    tenant.ConnectionString);
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

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    context.Response.StatusCode = 401;

                    await context.Response.WriteAsJsonAsync(
                        new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Authentication required."
                        });
                },

                OnForbidden = async context =>
                {
                    context.Response.StatusCode = 403;

                    await context.Response.WriteAsJsonAsync(
                        new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Access denied."
                        });
                }
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

        services.AddDbContext<TenantDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("TenantConnection"));
        });

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITenantDatabaseService, TenantDatabaseService>();

        services.AddScoped<ITenantRepository, TenantRepository>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        return services;
    }
}