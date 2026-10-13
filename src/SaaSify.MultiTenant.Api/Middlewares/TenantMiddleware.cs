using SaaSify.MultiTenant.Api.Responses;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Common;
using SaaSify.MultiTenant.Application.Common.Interfaces;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Api.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentUserService currentUserService,
        ITenantProvider tenantProvider,
        MasterDbContext masterDbContext)
    {
        if (currentUserService.IsAuthenticated
            && currentUserService.TenantId.HasValue)
        {
            var tenant =
                await masterDbContext.Tenants
                    .FindAsync(
                        currentUserService.TenantId.Value);

            if (tenant is null)
            {
                // Token is still valid but its tenant has been deleted.
                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                await context.Response.WriteAsJsonAsync(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Tenant not found."
                    });

                return;
            }

            tenantProvider.SetTenant(
                new TenantInfo
                {
                    TenantId = tenant.Id,
                    Identifier = tenant.TenantId,
                    ConnectionString = tenant.DbConnStr
                });
        }

        await _next(context);
    }
}
