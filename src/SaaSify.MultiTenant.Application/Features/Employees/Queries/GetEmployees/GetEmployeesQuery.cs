using MediatR;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Employees.Queries.GetEmployees;

public record GetEmployeesQuery()
    : IRequest<List<EmployeeResponseDto>>;