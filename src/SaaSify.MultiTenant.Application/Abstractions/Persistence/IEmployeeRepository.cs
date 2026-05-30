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
}