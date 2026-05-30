using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.CreateEmployee;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;
using SaaSify.MultiTenant.Application.Features.Employees.Queries.GetEmployeeById;
using SaaSify.MultiTenant.Application.Features.Employees.Queries.GetEmployees;
using SaaSify.MultiTenant.Application.Features.Employees.Queries.GetMyProfile;
using SaaSify.MultiTenant.Shared.Responses;

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

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Employee")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var result =
            await _mediator.Send(
                new GetMyProfileQuery());

        return Ok(
            ApiResponse<EmployeeResponseDto>
                .SuccessResponse(result));
    }
}