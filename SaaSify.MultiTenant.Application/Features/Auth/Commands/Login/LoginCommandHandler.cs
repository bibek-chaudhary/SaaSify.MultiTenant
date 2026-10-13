using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Auth.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IIdentityService identityService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;

        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ValidateUserAsync(
                request.Email,
                request.Password);

        if (!result.Success)
        {
            throw new UnauthorizedException(
                "Invalid credentials.");
        }

        var (token, expiresAtUtc) = await _jwtTokenGenerator.GenerateTokenAsync(
                result.UserId,
                result.Email,
                result.Role,
                result.TenantId);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        };
    }
}