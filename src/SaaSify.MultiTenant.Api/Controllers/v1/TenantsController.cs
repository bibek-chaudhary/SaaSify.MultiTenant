using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSify.MultiTenant.Application.Features.Tenants.Commands.CreateTenant;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;
using SaaSify.MultiTenant.Application.Features.Tenants.Queries.GetAllTenants;
using SaaSify.MultiTenant.Shared.Responses;

namespace SaaSify.MultiTenant.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "SuperAdminOnly")]
[Route("api/v{version:apiVersion}/tenants")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant( [FromBody] CreateTenantCommand command)
    {
        var response =await _mediator.Send(command);

        return Ok(ApiResponse<TenantResponseDto>.SuccessResponse(response,"Tenant created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _mediator.Send(new GetAllTenantsQuery());

        return Ok(ApiResponse<List<TenantResponseDto>>.SuccessResponse(response));
    }
}