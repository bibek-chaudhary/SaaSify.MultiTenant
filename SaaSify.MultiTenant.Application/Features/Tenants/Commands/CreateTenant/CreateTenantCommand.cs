using MediatR;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.CreateTenant;

public record CreateTenantCommand(string Name, string EmailAddress, string AdminPassword) : IRequest<TenantResponseDto>;