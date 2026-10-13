using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Employees.Queries.GetEmployees;

public sealed class GetEmployeesQueryHandler
    : IRequestHandler<GetEmployeesQuery, List<EmployeeResponseDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesQueryHandler(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<List<EmployeeResponseDto>> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var employees =
            await _employeeRepository.GetAllAsync(
                cancellationToken);

        return employees.Select(employee =>
            new EmployeeResponseDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                EmailAddress = employee.EmailAddress
            })
            .ToList();
    }
}