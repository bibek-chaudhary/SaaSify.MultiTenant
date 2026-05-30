using FluentValidation;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.UpdateTenant;

public sealed class UpdateTenantCommandValidator
    : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.Name) ||
                !string.IsNullOrWhiteSpace(x.EmailAddress))
            .WithMessage("At least one field must be provided.");

        When(x => !string.IsNullOrWhiteSpace(x.EmailAddress), () =>
        {
            RuleFor(x => x.EmailAddress!)
                .EmailAddress();
        });
    }
}