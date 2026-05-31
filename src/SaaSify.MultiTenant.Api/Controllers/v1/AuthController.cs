using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaaSify.MultiTenant.Application.Features.Auth.Commands.Login;
using SaaSify.MultiTenant.Application.Features.Auth.DTOs;
using SaaSify.MultiTenant.Api.Responses;

namespace SaaSify.MultiTenant.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var response =
            await _mediator.Send(command);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse( response, "Login successful."));
    }
}