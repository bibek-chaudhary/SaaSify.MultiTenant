using MediatR;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.DeleteTenant;

public sealed record DeleteTenantCommand(
    Guid Id)
    : IRequest<bool>;
