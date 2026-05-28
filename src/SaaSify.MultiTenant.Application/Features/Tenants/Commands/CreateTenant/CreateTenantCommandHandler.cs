using MediatR;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;
using SaaSify.MultiTenant.Application.Interfaces;
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
        var tenantIdentifier =
            GenerateTenantIdentifier();

        var connectionString =
            await _tenantDatabaseService
                .CreateTenantDatabaseAsync(
                    tenantIdentifier);

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),

            Name = request.Name,

            EmailAddress = request.EmailAddress,

            TenantId = tenantIdentifier,

            DbConnStr = connectionString,

            CreatedAtUtc = DateTime.UtcNow
        };

        await _tenantRepository.AddAsync(
            tenant,
            cancellationToken);

        await _identityService.CreateTenantAdminAsync(
            request.EmailAddress,
            tenant.Id,
            "Admin@123");

        return new TenantResponseDto
        {
            Id = tenant.Id,

            Name = tenant.Name,

            EmailAddress = tenant.EmailAddress,

            TenantId = tenant.TenantId
        };
    }

    private static string GenerateTenantIdentifier()
    {
        var random =
            Random.Shared.Next(1000, 9999);

        return random.ToString();
    }
}