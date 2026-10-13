using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler
    : IRequestHandler<UpdateTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;

    private readonly IIdentityService _identityService;

    public UpdateTenantCommandHandler(
        ITenantRepository tenantRepository,
        IIdentityService identityService)
    {
        _tenantRepository = tenantRepository;
        _identityService = identityService;
    }

    public async Task<bool> Handle(
        UpdateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenant =
            await _tenantRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(
                "Tenant not found.");
        }

        var previousEmail = tenant.EmailAddress;

        var emailChanged =
            !string.IsNullOrWhiteSpace(request.EmailAddress) &&
            !string.Equals(
                request.EmailAddress,
                previousEmail,
                StringComparison.OrdinalIgnoreCase);

        Guid adminUserId = default;

        if (emailChanged)
        {
            // The tenant email is the tenant admin's login, so keep them in sync.
            adminUserId =
                await _identityService.FindUserIdByEmailAsync(previousEmail)
                ?? throw new NotFoundException(
                    "Tenant admin user not found.");

            await _identityService.UpdateEmailAsync(
                adminUserId,
                request.EmailAddress!);

            tenant.EmailAddress = request.EmailAddress!;
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            tenant.Name = request.Name;
        }

        tenant.UpdatedAtUtc = DateTime.UtcNow;

        try
        {
            await _tenantRepository.UpdateAsync(
                tenant,
                cancellationToken);
        }
        catch when (emailChanged)
        {
            await _identityService.UpdateEmailAsync(
                adminUserId,
                previousEmail);
            throw;
        }

        return true;
    }
}
