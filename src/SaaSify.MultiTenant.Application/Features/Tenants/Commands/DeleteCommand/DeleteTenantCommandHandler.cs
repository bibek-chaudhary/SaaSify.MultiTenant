using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Database;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.DeleteCommand;

public class DeleteTenantCommandHandler
    : IRequestHandler<DeleteTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;


    public DeleteTenantCommandHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<bool> Handle(
        DeleteTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenant =
            await _tenantRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (tenant is null)
        {
            throw new ApplicationException(
                "Tenant not found.");
        }

        await _tenantRepository.DeleteAsync(
            tenant,
            cancellationToken);

        return true;
    }
}