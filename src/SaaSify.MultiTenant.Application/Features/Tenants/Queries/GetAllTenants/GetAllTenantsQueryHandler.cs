using MediatR;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;
using SaaSify.MultiTenant.Application.Interfaces;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Queries.GetAllTenants;

public class GetAllTenantsQueryHandler
    : IRequestHandler<
        GetAllTenantsQuery,
        List<TenantResponseDto>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetAllTenantsQueryHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<List<TenantResponseDto>> Handle(
        GetAllTenantsQuery request,
        CancellationToken cancellationToken)
    {
        return await _tenantRepository.GetAllAsync(
            cancellationToken);
    }
}