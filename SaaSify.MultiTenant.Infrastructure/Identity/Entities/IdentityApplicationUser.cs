using Microsoft.AspNetCore.Identity;

namespace SaaSify.MultiTenant.Infrastructure.Identity.Entities;

public class IdentityApplicationUser : IdentityUser<Guid>
{
    public Guid? TenantId { get; set; }
}