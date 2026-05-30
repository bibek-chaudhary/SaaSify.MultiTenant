using SaaSify.MultiTenant.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSify.MultiTenant.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{ 
    Task<Employee> AddAsync(
        Employee employee,
        CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(
        string emailAddress,
        CancellationToken cancellationToken);

    Task<List<Employee>> GetAllAsync(
    CancellationToken cancellationToken);

    Task<Employee?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken);

    Task<Employee?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Employee employee,
        CancellationToken cancellationToken);
}