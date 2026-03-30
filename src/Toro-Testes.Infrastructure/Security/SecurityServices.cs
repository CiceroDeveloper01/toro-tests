using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Constants;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Infrastructure.Authentication;

namespace Toro.Testes.Infrastructure.Security;

internal sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(Customer customer)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ApplicationConstants.Claims.CustomerId, customer.Id.ToString()),
            new Claim(ApplicationConstants.Claims.Email, customer.Email),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, customer.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_options.ExpirationHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? CustomerId =>
        Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue(ApplicationConstants.Claims.CustomerId), out var value)
            ? value
            : null;

    public string? Email => httpContextAccessor.HttpContext?.User.FindFirstValue(ApplicationConstants.Claims.Email);

    public string? Role => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
}
