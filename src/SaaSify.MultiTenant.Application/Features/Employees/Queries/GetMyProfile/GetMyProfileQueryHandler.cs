using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;

namespace SaaSify.MultiTenant.Application.Features.Employees.Queries.GetMyProfile;

public sealed class GetMyProfileQueryHandler
    : IRequestHandler<GetMyProfileQuery, EmployeeResponseDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    private readonly ICurrentUserService _currentUserService;

    public GetMyProfileQueryHandler(
        IEmployeeRepository employeeRepository,
        ICurrentUserService currentUserService)
    {
        _employeeRepository = employeeRepository;
        _currentUserService = currentUserService;
    }

    public async Task<EmployeeResponseDto> Handle(
        GetMyProfileQuery request,
        CancellationToken cancellationToken)
    {
        var employee =
            await _employeeRepository.GetByEmailAsync(
                _currentUserService.Email,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee profile not found.");
        }

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            EmailAddress = employee.EmailAddress
        };
    }
}