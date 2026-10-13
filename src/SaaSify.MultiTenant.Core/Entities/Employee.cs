using SaaSify.MultiTenant.Core.Common;

namespace SaaSify.MultiTenant.Core.Entities;

public class Employee : BaseEntity
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = default!;

    public string EmailAddress { get; set; } = default!;
}