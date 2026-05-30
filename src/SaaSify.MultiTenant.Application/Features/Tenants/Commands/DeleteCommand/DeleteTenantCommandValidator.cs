using FluentValidation;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.DeleteCommand;

public class DeleteTenantCommandValidator : AbstractValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}