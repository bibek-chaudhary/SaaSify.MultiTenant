using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Database;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.DeleteCommand;

public class DeleteTenantCommandHandler
    : IRequestHandler<DeleteTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantDatabaseService _tenantDatabaseService;

    public DeleteTenantCommandHandler(
        ITenantRepository tenantRepository,
        ITenantDatabaseService tenantDatabaseService)
    {
        _tenantRepository = tenantRepository;
        _tenantDatabaseService = tenantDatabaseService;
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
            throw new KeyNotFoundException(
                "Tenant not found.");
        }

        await _tenantRepository.DeleteAsync(
            tenant,
            cancellationToken);

        await _tenantDatabaseService.DeleteTenantDatabaseAsync(tenant.TenantId);

        return true;
    }
}