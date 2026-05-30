using MediatR;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.DeleteEmployee;

public sealed record DeleteEmployeeCommand(
    Guid Id)
    : IRequest<bool>;
