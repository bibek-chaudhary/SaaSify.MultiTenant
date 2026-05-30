using MediatR;

namespace SaaSify.MultiTenant.Application.Features.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FullName,
    string Email,
    string Password,
    string Role)
    : IRequest<bool>;