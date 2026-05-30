using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Queries.GetTenantById;

public class GetTenantByIdQueryHandler
    : IRequestHandler<
        GetTenantByIdQuery,
        TenantResponseDto>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantByIdQueryHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantResponseDto> Handle(
        GetTenantByIdQuery request,
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

        return new TenantResponseDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            EmailAddress = tenant.EmailAddress,
            TenantId = tenant.TenantId
        };
    }
}