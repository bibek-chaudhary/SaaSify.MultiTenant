using MediatR;
using SaaSify.MultiTenant.Application.Features.Auth.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;