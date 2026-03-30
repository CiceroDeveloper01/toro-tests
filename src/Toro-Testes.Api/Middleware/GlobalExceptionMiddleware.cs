using System.Text.Json;
using Toro.Testes.Api.Common;
using Toro.Testes.BuildingBlocks.Exceptions;
using Toro.Testes.BuildingBlocks.Helpers;

namespace Toro.Testes.Api.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context, ICorrelationContextAccessor correlationContextAccessor)
    {
        try
        {
            await next(context);
        }
        catch (AppException exception)
        {
            logger.LogWarning(exception, "Handled application exception with correlation {CorrelationId}", correlationContextAccessor.CorrelationId);
            context.Response.StatusCode = exception.StatusCode;
            context.Response.ContentType = "application/json";
            var response = new ApiErrorResponse(
                exception.Message,
                correlationContextAccessor.CorrelationId,
                exception is ValidationAppException validationException ? validationException.Errors : null);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception with correlation {CorrelationId}", correlationContextAccessor.CorrelationId);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var message = environment.IsDevelopment()
                ? $"An internal server error occurred. {exception.Message}"
                : "An internal server error occurred.";
            var response = new ApiErrorResponse(message, correlationContextAccessor.CorrelationId);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
