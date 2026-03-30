namespace Toro.Testes.Application.DTOs.Responses;

public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt, Guid CustomerId, string Role);
