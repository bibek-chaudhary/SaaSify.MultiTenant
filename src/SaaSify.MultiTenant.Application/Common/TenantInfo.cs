namespace SaaSify.MultiTenant.Application.Common;

public class TenantInfo
{
    public Guid TenantId { get; set; }

    public string Identifier { get; set; } = default!;

    public string ConnectionString { get; set; } = default!;
}
