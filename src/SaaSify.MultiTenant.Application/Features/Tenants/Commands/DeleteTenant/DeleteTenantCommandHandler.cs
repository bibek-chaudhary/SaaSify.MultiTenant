using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Database;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.DeleteTenant;

public class DeleteTenantCommandHandler
    : IRequestHandler<DeleteTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantDatabaseService _tenantDatabaseService;
    private readonly IIdentityService _identityService;

    public DeleteTenantCommandHandler(
        ITenantRepository tenantRepository,
        ITenantDatabaseService tenantDatabaseService,
        IIdentityService identityService)
    {
        _tenantRepository = tenantRepository;
        _tenantDatabaseService = tenantDatabaseService;
        _identityService = identityService;
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
            throw new NotFoundException(
                "Tenant not found.");
        }

        // Revoke logins first so no one can reach a half-deleted tenant.
        await _identityService.DeleteUsersByTenantAsync(tenant.Id);

        await _tenantRepository.DeleteAsync(
            tenant,
            cancellationToken);

        await _tenantDatabaseService.DeleteTenantDatabaseAsync(tenant.TenantId);

        return true;
    }
}
