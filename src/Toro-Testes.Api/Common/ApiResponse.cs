namespace Toro.Testes.Api.Common;

public sealed record ApiResponse<T>(string Message, T Data, string? CorrelationId = null);

public sealed record ApiErrorResponse(string Message, string CorrelationId, IReadOnlyDictionary<string, string[]>? Errors = null);
