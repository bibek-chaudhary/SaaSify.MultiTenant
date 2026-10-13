using MediatR;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.UpdateTenant;

public sealed record UpdateTenantCommand(
    Guid Id,
    string? Name,
    string? EmailAddress)
    : IRequest<bool>;