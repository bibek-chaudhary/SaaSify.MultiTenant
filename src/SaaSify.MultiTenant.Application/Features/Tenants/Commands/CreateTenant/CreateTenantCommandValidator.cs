using FluentValidation;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandValidator
    : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress();
    }
}