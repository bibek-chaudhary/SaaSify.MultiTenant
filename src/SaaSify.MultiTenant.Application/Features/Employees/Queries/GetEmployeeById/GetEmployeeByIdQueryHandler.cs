using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Employees.Queries.GetEmployeeById;

public sealed class GetEmployeeByIdQueryHandler
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponseDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByIdQueryHandler(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeResponseDto> Handle(
        GetEmployeeByIdQuery request,
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

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            EmailAddress = employee.EmailAddress
        };
    }
}