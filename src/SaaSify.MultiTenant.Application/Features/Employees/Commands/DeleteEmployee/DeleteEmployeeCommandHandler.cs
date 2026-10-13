using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.DeleteEmployee;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;

public sealed class DeleteEmployeeCommandHandler
    : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;

    private readonly IIdentityService _identityService;

    public DeleteEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IIdentityService identityService)
    {
        _employeeRepository = employeeRepository;
        _identityService = identityService;
    }

    public async Task<bool> Handle(
        DeleteEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee =
            await _employeeRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }

        // Remove the login first: a leftover employee row is harmless,
        // a leftover login for a deleted employee is not.
        await _identityService.DeleteUserAsync(employee.UserId);

        await _employeeRepository.DeleteAsync(
            employee,
            cancellationToken);

        return true;
    }
}
