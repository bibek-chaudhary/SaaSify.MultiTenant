using FluentValidation;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.DeleteEmployee;

public sealed class DeleteEmployeeCommandValidator
    : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}