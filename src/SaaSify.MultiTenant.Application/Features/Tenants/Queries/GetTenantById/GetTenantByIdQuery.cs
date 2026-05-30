using MediatR;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Queries.GetTenantById;

public sealed record GetTenantByIdQuery(
    Guid Id)
    : IRequest<TenantResponseDto>;