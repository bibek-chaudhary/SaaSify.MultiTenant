using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;
using SaaSify.MultiTenant.Core.Entities;

namespace SaaSify.MultiTenant.Application.Abstractions.Persistence;

public interface ITenantRepository
{
    Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken);

    Task<List<TenantResponseDto>> GetAllAsync(
        CancellationToken cancellationToken);
}