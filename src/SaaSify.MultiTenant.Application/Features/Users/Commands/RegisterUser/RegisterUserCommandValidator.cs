using FluentValidation;
using SaaSify.MultiTenant.Core.Constants;

namespace SaaSify.MultiTenant.Application.Features.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.Role)
            .Must(role =>
                role == Roles.Admin ||
                role == Roles.Employee)
            .WithMessage("Role must be Admin or Employee.");
    }
}