using MediatR;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.CreateEmployee;

public sealed record CreateEmployeeCommand(
    string FullName,
    string EmailAddress,
    string Password)
    : IRequest<EmployeeResponseDto>;