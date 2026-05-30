using Microsoft.EntityFrameworkCore;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Core.Entities;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository
    : IEmployeeRepository
{
    private readonly TenantDbContext _context;

    public EmployeeRepository(
        TenantDbContext context)
    {
        _context = context;
    }

    public async Task<Employee> AddAsync(
        Employee employee,
        CancellationToken cancellationToken)
    {
        await _context.Employees.AddAsync(
            employee,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return employee;
    }

    public async Task<bool> EmailExistsAsync(
        string emailAddress,
        CancellationToken cancellationToken)
    {
        return await _context.Employees
            .AnyAsync(
                x => x.EmailAddress == emailAddress,
                cancellationToken);
    }

    public async Task<List<Employee>> GetAllAsync(
    CancellationToken cancellationToken)
    {
        return await _context.Employees
            .OrderBy(x => x.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Employee?> GetByEmailAsync(
        string emailAddress,
        CancellationToken cancellationToken)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(
                x => x.EmailAddress == emailAddress,
                cancellationToken);
    }
}