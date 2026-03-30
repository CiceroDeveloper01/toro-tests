namespace Toro.Testes.BuildingBlocks.Exceptions;

public sealed class ValidationAppException(IReadOnlyDictionary<string, string[]> errors)
    : AppException("One or more validation failures have occurred.", 400)
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
