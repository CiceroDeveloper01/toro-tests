using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Constants;
using Toro.Testes.Contracts.Common;
using Toro.Testes.Contracts.Events;
using Toro.Testes.Infrastructure.Messaging;

namespace Toro.Testes.Worker.Consumers;

public sealed class InvestmentOrderCreatedConsumer(
    IServiceScopeFactory scopeFactory,
    IRabbitMqConnectionProvider connectionProvider,
    ILogger<InvestmentOrderCreatedConsumer> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => ExecuteConsumerAsync(stoppingToken);

    private async Task ExecuteConsumerAsync(CancellationToken stoppingToken)
    {
        using var connection = connectionProvider.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(ApplicationConstants.Messaging.Exchange, ExchangeType.Topic, durable: true);
        channel.QueueDeclare(ApplicationConstants.Messaging.OrderCreatedQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(ApplicationConstants.Messaging.OrderCreatedQueue, ApplicationConstants.Messaging.Exchange, ApplicationConstants.Messaging.OrderCreatedRoutingKey);
        channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, ea) =>
        {
            using var scope = scopeFactory.CreateScope();
            var processingService = scope.ServiceProvider.GetRequiredService<IInvestmentOrderProcessingService>();

            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var envelope = JsonSerializer.Deserialize<MessageEnvelope<InvestmentOrderCreatedIntegrationEvent>>(body)
                    ?? throw new InvalidOperationException("Invalid message payload.");

                await processingService.ProcessAsync(envelope, stoppingToken);
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error processing RabbitMQ message {MessageId}", ea.BasicProperties.MessageId);
                var headers = ea.BasicProperties.Headers;
                var retries = headers is not null && headers.TryGetValue("x-retry-count", out var retryValue)
                    ? Convert.ToInt32(Encoding.UTF8.GetString((byte[])retryValue))
                    : 0;

                if (retries >= 3)
                {
                    channel.BasicPublish(string.Empty, ApplicationConstants.Messaging.DeadLetterQueue, ea.BasicProperties, ea.Body);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    var properties = channel.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object> { ["x-retry-count"] = Encoding.UTF8.GetBytes((retries + 1).ToString()) };
                    properties.MessageId = ea.BasicProperties.MessageId;
                    properties.CorrelationId = ea.BasicProperties.CorrelationId;
                    properties.Type = ea.BasicProperties.Type;
                    channel.BasicPublish(ApplicationConstants.Messaging.Exchange, ApplicationConstants.Messaging.OrderCreatedRoutingKey, properties, ea.Body);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            }
        };

        channel.BasicConsume(ApplicationConstants.Messaging.OrderCreatedQueue, false, consumer);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            // Expected during host shutdown.
        }
    }
}
