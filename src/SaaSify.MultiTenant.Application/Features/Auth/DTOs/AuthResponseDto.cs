namespace SaaSify.MultiTenant.Application.Features.Auth.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = default!;

    public DateTime ExpiresAtUtc { get; set; }
}