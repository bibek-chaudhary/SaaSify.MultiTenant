namespace SaaSify.MultiTenant.Application.Features.Tenants.DTOs;

public class TenantResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string EmailAddress { get; set; } = default!;

    public string TenantId { get; set; } = default!;
}