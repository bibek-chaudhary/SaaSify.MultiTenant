using MediatR;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid Id)
    : IRequest<EmployeeResponseDto>;