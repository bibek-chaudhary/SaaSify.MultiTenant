using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSify.MultiTenant.Application.Features.Tenants.Commands.CreateTenant;
using SaaSify.MultiTenant.Application.Features.Tenants.Commands.DeleteTenant;
using SaaSify.MultiTenant.Application.Features.Tenants.Commands.UpdateTenant;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;
using SaaSify.MultiTenant.Application.Features.Tenants.Queries.GetAllTenants;
using SaaSify.MultiTenant.Application.Features.Tenants.Queries.GetTenantById;
using SaaSify.MultiTenant.Api.Responses;

namespace SaaSify.MultiTenant.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "SuperAdminOnly")]
[Route("api/v{version:apiVersion}/tenants")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant(
        [FromBody] CreateTenantCommand command)
    {
        var response =
            await _mediator.Send(command);

        return Ok(
            ApiResponse<TenantResponseDto>
                .SuccessResponse(
                    response,
                    "Tenant created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response =
            await _mediator.Send(
                new GetAllTenantsQuery());

        return Ok(
            ApiResponse<List<TenantResponseDto>>
                .SuccessResponse(response));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var response =
            await _mediator.Send(
                new GetTenantByIdQuery(id));

        return Ok(
            ApiResponse<TenantResponseDto>
                .SuccessResponse(response));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTenantCommand command)
    {
        var response =
            await _mediator.Send(
                command with { Id = id });

        return Ok(
            ApiResponse<bool>
                .SuccessResponse(
                    response,
                    "Tenant updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        var response =
            await _mediator.Send(
                new DeleteTenantCommand(id));

        return Ok(
            ApiResponse<bool>
                .SuccessResponse(
                    response,
                    "Tenant deleted successfully."));
    }
}