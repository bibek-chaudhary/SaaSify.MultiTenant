using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Employees.DTOs;
using SaaSify.MultiTenant.Core.Constants;
using SaaSify.MultiTenant.Core.Entities;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandHandler
    : IRequestHandler<CreateEmployeeCommand, EmployeeResponseDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    private readonly IIdentityService _identityService;

    private readonly ICurrentUserService _currentUserService;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _employeeRepository = employeeRepository;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<EmployeeResponseDto> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.TenantId.HasValue)
        {
            throw new UnauthorizedException(
                "Tenant context not found.");
        }

        var exists =
            await _employeeRepository
                .EmailExistsAsync(
                    request.EmailAddress,
                    cancellationToken);

        if (exists)
        {
            throw new ConflictException(
                "Employee with this email already exists.");
        }

        var userId =
            await _identityService.RegisterUserAsync(
                request.EmailAddress,
                request.Password,
                Roles.Employee,
                _currentUserService.TenantId.Value);

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = request.FullName,
            EmailAddress = request.EmailAddress,
            CreatedAtUtc = DateTime.UtcNow
        };

        try
        {
            await _employeeRepository.AddAsync(
                employee,
                cancellationToken);
        }
        catch
        {
            // Identity lives in the master DB, so roll it back by hand.
            await _identityService.DeleteUserAsync(userId);
            throw;
        }

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            EmailAddress = employee.EmailAddress
        };
    }
}
