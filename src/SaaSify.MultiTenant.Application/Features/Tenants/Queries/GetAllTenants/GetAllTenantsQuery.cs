using MediatR;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Queries.GetAllTenants;

public record GetAllTenantsQuery()
    : IRequest<List<TenantResponseDto>>;