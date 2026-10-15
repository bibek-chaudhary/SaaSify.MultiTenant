using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using SaaSify.MultiTenant.Api.Responses;
using SaaSify.MultiTenant.Infrastructure.Authentication;
using System.Threading.RateLimiting;

namespace SaaSify.MultiTenant.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddRateLimiter(options =>
        {
            // Per-IP cap on login attempts; per-account lockout is handled by Identity.
            options.AddPolicy(RateLimitPolicies.Login, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode =
                    StatusCodes.Status429TooManyRequests;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString();
                }

                await context.HttpContext.Response.WriteAsJsonAsync(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Too many requests. Try again later."
                    },
                    cancellationToken);
            };
        });

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);

            options.AssumeDefaultVersionWhenUnspecified = true;

            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";

            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SaaSify MultiTenant API",
                Version = "v1"
            });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token"
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var validator =
                            context.HttpContext.RequestServices
                                .GetRequiredService<JwtSecurityStampValidator>();

                        if (!await validator.ValidateAsync(context.Principal!))
                        {
                            context.Fail("Token has been revoked.");
                        }
                    },

                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 401;

                        await context.Response.WriteAsJsonAsync(
                            new ApiResponse<object>
                            {
                                Success = false,
                                Message = context.AuthenticateFailure is null
                                    ? "Authentication required."
                                    : "Invalid, expired or revoked token."
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

        return services;
    }
}
