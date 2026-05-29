using Microsoft.AspNetCore.Http;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SaaSify.MultiTenant.Infrastructure.Authentication;

public class  CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get { 
            var value = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return value is null ? Guid.Empty : Guid.Parse(value);
        }
    }

    public string Email =>
       _httpContextAccessor.HttpContext?
           .User
           .FindFirstValue(ClaimTypes.Email)
       ?? string.Empty;

    public string Role =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.Role)
        ?? string.Empty;

    public Guid? TenantId
    {
        get
        {
            var value =
                _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst("tenantId")
                    ?.Value;

            return value is null
                ? null
                : Guid.Parse(value);
        }
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?
            .User
            .Identity?
            .IsAuthenticated
        ?? false;
}