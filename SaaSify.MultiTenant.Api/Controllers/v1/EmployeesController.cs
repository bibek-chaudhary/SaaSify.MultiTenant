using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.CreateEmployee;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.DeleteEmployee;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;
using SaaSify.MultiTenant.Application.Features.Employees.Queries.GetEmployeeById;
using SaaSify.MultiTenant.Application.Features.Employees.Queries.GetEmployees;
using SaaSify.MultiTenant.Application.Features.Employees.Queries.GetMyProfile;
using SaaSify.MultiTenant.Core.Constants;
using SaaSify.MultiTenant.Api.Responses;

namespace SaaSify.MultiTenant.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(
        CreateEmployeeCommand command)
    {
        var result =
            await _mediator.Send(command);

        return Ok(
            ApiResponse<EmployeeResponseDto>
                .SuccessResponse(
                    result,
                    "Employee created successfully."));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var result =
            await _mediator.Send(
                new GetEmployeesQuery());

        return Ok(
            ApiResponse<List<EmployeeResponseDto>>
                .SuccessResponse(result));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEmployeeById(
    Guid id)
    {
        var result =
            await _mediator.Send(
                new GetEmployeeByIdQuery(id));

        return Ok(
            ApiResponse<EmployeeResponseDto>
                .SuccessResponse(result));
    }

    [Authorize(Policy = "EmployeeOnly")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var response =
            await _mediator.Send(
                new GetMyProfileQuery());

        return Ok(
            ApiResponse<EmployeeResponseDto>
                .SuccessResponse(
                    response,
                    "Employee profile retrieved."));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateEmployeeCommand request)
    {
        var result = await _mediator.Send(request with { Id = id });

        return Ok(  
            ApiResponse<bool>.SuccessResponse(
                result,
                "Employee updated."));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
    Guid id)
    {
        await _mediator.Send(
            new DeleteEmployeeCommand(id));

        return Ok(
            ApiResponse<bool>.SuccessResponse(
                true,
                "Employee deleted."));
    }
}