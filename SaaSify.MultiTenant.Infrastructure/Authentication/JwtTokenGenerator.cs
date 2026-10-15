using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace SaaSify.MultiTenant.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    private readonly UserManager<IdentityApplicationUser> _userManager;

    public JwtTokenGenerator(
        IOptions<JwtSettings> jwtOptions,
        UserManager<IdentityApplicationUser> userManager)
    {
        _jwtSettings = jwtOptions.Value;
        _userManager = userManager;
    }

    public async Task<(string Token, DateTime ExpiresAtUtc)> GenerateTokenAsync(
        Guid userId,
        string email,
        string role,
        Guid? tenantId)
    {
        var user =
            await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new InvalidOperationException(
                $"User '{userId}' not found.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),

            new(ClaimTypes.Email, email),

            new(ClaimTypes.Role, role),

            new(JwtSecurityStampValidator.ClaimType,
                await _userManager.GetSecurityStampAsync(user))
        };

        if (tenantId.HasValue)
        {
            claims.Add(
                new Claim(
                    "tenantId",
                    tenantId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiresAtUtc =
            DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var jwt =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return (jwt, expiresAtUtc);
    }
}