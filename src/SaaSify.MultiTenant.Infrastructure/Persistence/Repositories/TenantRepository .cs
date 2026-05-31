using Microsoft.EntityFrameworkCore;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Features.Tenants.DTOs;
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

    public async Task<Tenant?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task UpdateAsync(
        Tenant tenant,
        CancellationToken cancellationToken)
    {
        _context.Tenants.Update(tenant);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Tenant tenant,
        CancellationToken cancellationToken)
    {
        _context.Tenants.Remove(tenant);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        string emailAddress,
        CancellationToken cancellationToken)
    {
        return await _context.Tenants
            .AnyAsync(
                x => x.EmailAddress == emailAddress,
                cancellationToken);
    }
}