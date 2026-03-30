namespace Toro.Testes.BuildingBlocks.Exceptions;

public sealed class NotFoundException(string message) : AppException(message, 404);
