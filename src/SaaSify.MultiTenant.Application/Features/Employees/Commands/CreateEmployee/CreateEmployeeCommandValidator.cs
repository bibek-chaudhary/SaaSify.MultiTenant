using FluentValidation;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandValidator
    : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress();
    }
}