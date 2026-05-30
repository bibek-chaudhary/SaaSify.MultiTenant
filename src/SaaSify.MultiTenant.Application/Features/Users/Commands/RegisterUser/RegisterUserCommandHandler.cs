using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Core.Constants;
using SaaSify.MultiTenant.Core.Entities;

namespace SaaSify.MultiTenant.Application.Features.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly IIdentityService _identityService;

    private readonly ICurrentUserService _currentUserService;

    private readonly IEmployeeRepository _employeeRepository;

    public RegisterUserCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        IEmployeeRepository employeeRepository)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
        _employeeRepository = employeeRepository;
    }

    public async Task<bool> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.TenantId.HasValue)
        {
            throw new ApplicationException(
                "Tenant context not found.");
        }

        await _identityService.RegisterUserAsync(
            request.Email,
            request.Password,
            request.Role,
            _currentUserService.TenantId.Value);

        if (request.Role == Roles.Employee)
        {
            var employee =
                new Employee
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName,
                    EmailAddress = request.Email,
                    CreatedAtUtc = DateTime.UtcNow
                };

            await _employeeRepository.AddAsync(
                employee,
                cancellationToken);
        }

        return true;
    }
}