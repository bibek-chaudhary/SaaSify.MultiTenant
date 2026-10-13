using MediatR;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;

public sealed record UpdateEmployeeCommand(
    Guid Id,
    string? FullName,
    string? EmailAddress)
    : IRequest<bool>;