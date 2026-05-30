using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;
using SaaSify.MultiTenant.Core.Entities;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandHandler
    : IRequestHandler<CreateEmployeeCommand, EmployeeResponseDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeResponseDto> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var exists =
            await _employeeRepository
                .EmailExistsAsync(
                    request.EmailAddress,
                    cancellationToken);

        if (exists)
        {
            throw new ApplicationException(
                "Employee email already exists.");
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            EmailAddress = request.EmailAddress,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _employeeRepository.AddAsync(
            employee,
            cancellationToken);

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            EmailAddress = employee.EmailAddress
        };
    }
}