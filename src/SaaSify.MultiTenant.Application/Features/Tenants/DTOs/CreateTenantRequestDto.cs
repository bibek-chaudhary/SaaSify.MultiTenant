namespace SaaSify.MultiTenant.Application.Features.Tenants.DTOs;

public class CreateTenantRequestDto
{
    public string Name { get; set; } = default!;

    public string EmailAddress { get; set; } = default!;

    public string AdminPassword { get; set; } = default!;
}