using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;

namespace SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;

public sealed class UpdateEmployeeCommandHandler
    : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;

    private readonly IIdentityService _identityService;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IIdentityService identityService)
    {
        _employeeRepository = employeeRepository;
        _identityService = identityService;
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
            throw new NotFoundException(
                "Employee not found.");
        }

        var previousEmail = employee.EmailAddress;

        var emailChanged =
            !string.IsNullOrWhiteSpace(request.EmailAddress) &&
            !string.Equals(
                request.EmailAddress,
                previousEmail,
                StringComparison.OrdinalIgnoreCase);

        if (emailChanged)
        {
            // Identity enforces global email uniqueness, so it goes first.
            await _identityService.UpdateEmailAsync(
                employee.UserId,
                request.EmailAddress!);

            employee.EmailAddress = request.EmailAddress!;
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            employee.FullName = request.FullName;
        }

        employee.UpdatedAtUtc = DateTime.UtcNow;

        try
        {
            await _employeeRepository.UpdateAsync(
                employee,
                cancellationToken);
        }
        catch when (emailChanged)
        {
            await _identityService.UpdateEmailAsync(
                employee.UserId,
                previousEmail);
            throw;
        }

        return true;
    }
}
