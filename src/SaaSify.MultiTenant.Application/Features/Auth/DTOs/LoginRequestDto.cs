namespace SaaSify.MultiTenant.Application.Features.Auth.DTOs;

public class LoginRequestDto
{
    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
}