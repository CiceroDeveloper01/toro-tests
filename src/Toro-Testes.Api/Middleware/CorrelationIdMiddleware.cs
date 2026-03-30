using Toro.Testes.BuildingBlocks.Helpers;

namespace Toro.Testes.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICorrelationContextAccessor correlationContextAccessor)
    {
        var correlationId = context.Request.Headers.TryGetValue("X-Correlation-Id", out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.ToString()
            : Guid.NewGuid().ToString("N");

        correlationContextAccessor.CorrelationId = correlationId;
        context.Response.Headers["X-Correlation-Id"] = correlationId;
        await next(context);
    }
}
