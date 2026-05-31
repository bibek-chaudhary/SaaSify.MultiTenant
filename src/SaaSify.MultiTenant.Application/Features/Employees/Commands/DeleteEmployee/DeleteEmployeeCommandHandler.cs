using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.DeleteEmployee;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;

public sealed class DeleteEmployeeCommandHandler
    : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;

    public DeleteEmployeeCommandHandler(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
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

        await _employeeRepository.DeleteAsync(
            employee,
            cancellationToken);

        return true;
    }
}