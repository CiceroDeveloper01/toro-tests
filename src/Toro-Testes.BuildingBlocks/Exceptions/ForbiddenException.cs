namespace Toro.Testes.BuildingBlocks.Exceptions;

public sealed class ForbiddenException(string message = "Access denied.") : AppException(message, 403);
