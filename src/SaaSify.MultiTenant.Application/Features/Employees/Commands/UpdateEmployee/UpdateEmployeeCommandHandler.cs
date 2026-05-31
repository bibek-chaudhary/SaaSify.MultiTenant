using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;

public sealed class UpdateEmployeeCommandHandler
    : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<bool> Handle(
    UpdateEmployeeCommand request,
    CancellationToken cancellationToken)
    {
        var employee =
            await _employeeRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (employee is null)
        {
            throw new KeyNotFoundException(
                "Employee not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            employee.FullName = request.FullName;
        }

        if (!string.IsNullOrWhiteSpace(request.EmailAddress))
        {
            employee.EmailAddress = request.EmailAddress;
        }

        employee.UpdatedAtUtc = DateTime.UtcNow;

        await _employeeRepository.UpdateAsync(
            employee,
            cancellationToken);

        return true;
    }
}