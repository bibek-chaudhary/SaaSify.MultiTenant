using MediatR;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Database;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;
using SaaSify.MultiTenant.Core.Entities;

namespace SaaSify.MultiTenant.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler
    : IRequestHandler<CreateTenantCommand, TenantResponseDto>
{
    private readonly ITenantRepository _tenantRepository;

    private readonly ITenantDatabaseService _tenantDatabaseService;

    private readonly IIdentityService _identityService;

    public CreateTenantCommandHandler(
        ITenantRepository tenantRepository,
        ITenantDatabaseService tenantDatabaseService,
        IIdentityService identityService)
    {
        _tenantRepository = tenantRepository;

        _tenantDatabaseService = tenantDatabaseService;

        _identityService = identityService;
    }

    public async Task<TenantResponseDto> Handle(
        CreateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var emailExists = await _tenantRepository
            .ExistsByEmailAsync(request.EmailAddress, cancellationToken);

        if (emailExists)
            throw new ConflictException(
                $"A tenant with email '{request.EmailAddress}' already exists.");

        // The tenant email becomes the admin's login, which must be globally unique.
        // Check up front so we don't create a database we'd immediately drop.
        var adminExists =
            await _identityService.FindUserIdByEmailAsync(request.EmailAddress);

        if (adminExists.HasValue)
            throw new ConflictException(
                $"A user with email '{request.EmailAddress}' already exists.");

        var tenantIdentifier =
            await GenerateTenantIdentifierAsync(cancellationToken);

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            EmailAddress = request.EmailAddress,
            TenantId = tenantIdentifier,
            CreatedAtUtc = DateTime.UtcNow
        };

        // CreateTenantDatabaseAsync cleans up after itself if it fails, so only
        // drop the database here once we know this request created it.
        tenant.DbConnStr =
            await _tenantDatabaseService
                .CreateTenantDatabaseAsync(tenantIdentifier);

        var tenantAdded = false;

        try
        {
            await _tenantRepository.AddAsync(tenant, cancellationToken);

            tenantAdded = true;

            await _identityService.CreateTenantAdminAsync(
                request.EmailAddress,
                tenant.Id,
                request.AdminPassword);
        }
        catch
        {
            // Undo everything; cleanup failures must not hide the original error.
            if (tenantAdded)
            {
                await TryAsync(() =>
                    _tenantRepository.DeleteAsync(tenant, CancellationToken.None));
            }

            await TryAsync(() =>
                _tenantDatabaseService.DeleteTenantDatabaseAsync(tenantIdentifier));

            throw;
        }

        return new TenantResponseDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            EmailAddress = tenant.EmailAddress,
            TenantId = tenant.TenantId
        };
    }

    private const int MaxIdentifierAttempts = 10;

    private async Task<string> GenerateTenantIdentifierAsync(
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxIdentifierAttempts; attempt++)
        {
            var identifier =
                Random.Shared.Next(1000, 10000).ToString();

            var taken =
                await _tenantRepository.ExistsByIdentifierAsync(
                    identifier,
                    cancellationToken);

            if (!taken)
                return identifier;
        }

        throw new ConflictException(
            "Could not allocate a unique tenant identifier. Please try again.");
    }

    private static async Task TryAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch
        {
            // Best-effort cleanup.
        }
    }
}