using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Toro.Testes.BuildingBlocks.Exceptions;
using Toro.Testes.BuildingBlocks.Helpers;

namespace Toro.Testes.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var errors = failures
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());

        if (errors.Count != 0)
        {
            throw new ValidationAppException(errors);
        }

        return await next();
    }
}

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    ICorrelationContextAccessor correlationContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {RequestName} with correlation {CorrelationId}: {@Request}", typeof(TRequest).Name, correlationContextAccessor.CorrelationId, request);
        var response = await next();
        logger.LogInformation("Handled {RequestName} with correlation {CorrelationId}", typeof(TRequest).Name, correlationContextAccessor.CorrelationId);
        return response;
    }
}
