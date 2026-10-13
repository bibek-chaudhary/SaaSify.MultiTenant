using FluentValidation;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;

public sealed class UpdateEmployeeCommandValidator
    : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.FullName) ||
                !string.IsNullOrWhiteSpace(x.EmailAddress))
            .WithMessage("At least one field must be provided.");

        When(x => !string.IsNullOrWhiteSpace(x.EmailAddress), () =>
        {
            RuleFor(x => x.EmailAddress!)
                .EmailAddress();
        });
    }
}