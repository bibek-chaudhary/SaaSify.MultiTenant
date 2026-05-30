using MediatR;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.DeleteCommand;

public sealed record DeleteTenantCommand(
    Guid Id)
    : IRequest<bool>;
