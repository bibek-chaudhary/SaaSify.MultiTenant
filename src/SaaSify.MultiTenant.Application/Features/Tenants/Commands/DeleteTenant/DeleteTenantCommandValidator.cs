using FluentValidation;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.DeleteTenant;

public class DeleteTenantCommandValidator : AbstractValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
