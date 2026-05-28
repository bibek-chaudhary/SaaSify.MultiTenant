using SaaSify.MultiTenant.Core.Common;

namespace SaaSify.MultiTenant.Core.Entities;

public class ApplicationUser : BaseEntity
{
    public string EmailAddress { get; set; } = default!;

    public Guid? TenantId { get; set; }
}