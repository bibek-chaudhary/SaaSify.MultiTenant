using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Common.Interfaces;

namespace SaaSify.MultiTenant.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(
        [FromServices] ITenantProvider tenantProvider,
        [FromServices] ICurrentUserService currentUser)
    {
        var tenant =
            tenantProvider.GetCurrentTenant();

        return Ok(new
        {
            currentUser.UserId,
            currentUser.Email,
            currentUser.Role,
            currentUser.TenantId,
            TenantResolved = tenant is not null,
            ConnectionString =
                tenant?.ConnectionString
        });
    }
}