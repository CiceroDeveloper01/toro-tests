using Microsoft.Extensions.Diagnostics.HealthChecks;
using Toro.Testes.Infrastructure.Messaging;

namespace Toro.Testes.Infrastructure.Health;

internal sealed class RabbitMqHealthCheck(IRabbitMqConnectionProvider connectionProvider) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionProvider.CreateConnection();
            return Task.FromResult(connection.IsOpen
                ? HealthCheckResult.Healthy("RabbitMQ connection is available.")
                : HealthCheckResult.Unhealthy("RabbitMQ connection is closed."));
        }
        catch (Exception exception)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ is unavailable.", exception));
        }
    }
}
