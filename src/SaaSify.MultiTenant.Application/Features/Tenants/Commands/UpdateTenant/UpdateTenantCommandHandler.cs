using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler
    : IRequestHandler<UpdateTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;


    public UpdateTenantCommandHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<bool> Handle(
        UpdateTenantCommand request,
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

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            tenant.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.EmailAddress))
        {
            tenant.EmailAddress = request.EmailAddress;
        }

        tenant.UpdatedAtUtc = DateTime.UtcNow;

        await _tenantRepository.UpdateAsync(
            tenant,
            cancellationToken);

        return true;
    }
}