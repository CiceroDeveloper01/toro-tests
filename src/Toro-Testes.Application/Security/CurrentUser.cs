namespace Toro.Testes.Application.Security;

public sealed record CurrentUser(Guid CustomerId, string Email, string Role);
