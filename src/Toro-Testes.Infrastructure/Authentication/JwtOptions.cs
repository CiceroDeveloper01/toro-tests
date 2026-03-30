namespace Toro.Testes.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "Toro-Testes";
    public string Audience { get; init; } = "Toro-Testes-Clients";
    public string SecretKey { get; init; } = "super-secret-key-for-local-development-only-123456";
    public int ExpirationHours { get; init; } = 2;
}
