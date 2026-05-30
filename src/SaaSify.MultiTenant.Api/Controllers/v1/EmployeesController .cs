using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.CreateEmployee;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;
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
}