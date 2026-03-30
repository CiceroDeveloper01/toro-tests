namespace Toro.Testes.BuildingBlocks.Exceptions;

public sealed class UnauthorizedException(string message = "Authentication failed.") : AppException(message, 401);
