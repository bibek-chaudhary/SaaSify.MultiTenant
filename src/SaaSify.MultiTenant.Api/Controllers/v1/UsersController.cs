using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSify.MultiTenant.Application.Features.Users.Commands.RegisterUser;
using SaaSify.MultiTenant.Core.Constants;
using SaaSify.MultiTenant.Shared.Responses;

namespace SaaSify.MultiTenant.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Register(
        RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(
            ApiResponse<bool>.SuccessResponse(
                result,
                result ? "User registered successfully." : "User registration failed."));
    }
}