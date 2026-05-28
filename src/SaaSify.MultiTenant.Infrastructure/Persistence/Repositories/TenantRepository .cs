using Microsoft.EntityFrameworkCore;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;
using SaaSify.MultiTenant.Application.Interfaces;
using SaaSify.MultiTenant.Core.Entities;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly MasterDbContext _context;

    public TenantRepository(
        MasterDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken)
    {
        _context.Tenants.Add(tenant);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<List<TenantResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Tenants
            .Select(x => new TenantResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                EmailAddress = x.EmailAddress,
                TenantId = x.TenantId
            })
            .ToListAsync(cancellationToken);
    }
}