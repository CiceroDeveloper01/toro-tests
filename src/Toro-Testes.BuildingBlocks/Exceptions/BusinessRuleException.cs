namespace Toro.Testes.BuildingBlocks.Exceptions;

public sealed class BusinessRuleException(string message) : AppException(message, 422);
