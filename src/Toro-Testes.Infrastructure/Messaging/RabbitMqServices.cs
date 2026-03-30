using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Abstractions;
using Toro.Testes.BuildingBlocks.Constants;
using Toro.Testes.Contracts.Common;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Infrastructure.Data;

namespace Toro.Testes.Infrastructure.Messaging;

public interface IRabbitMqConnectionProvider
{
    IConnection CreateConnection();
}

internal sealed class RabbitMqConnectionProvider(IOptions<RabbitMqOptions> options) : IRabbitMqConnectionProvider
{
    private readonly RabbitMqOptions _options = options.Value;

    public IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true
        };

        return factory.CreateConnection();
    }
}

internal sealed class RabbitMqMessageBusPublisher(
    IRabbitMqConnectionProvider connectionProvider,
    ILogger<RabbitMqMessageBusPublisher> logger)
    : IMessageBusPublisher
{
    public Task PublishAsync<T>(MessageEnvelope<T> envelope, string routingKey, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(ApplicationConstants.Messaging.Exchange, ExchangeType.Topic, durable: true);
        channel.QueueDeclare(ApplicationConstants.Messaging.OrderCreatedQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueDeclare(ApplicationConstants.Messaging.DeadLetterQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(ApplicationConstants.Messaging.OrderCreatedQueue, ApplicationConstants.Messaging.Exchange, ApplicationConstants.Messaging.OrderCreatedRoutingKey);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.MessageId = envelope.MessageId.ToString();
        properties.CorrelationId = envelope.CorrelationId;
        properties.Type = envelope.EventName;

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope));
        channel.BasicPublish(ApplicationConstants.Messaging.Exchange, routingKey, properties, body);
        logger.LogInformation("Published message {MessageId} for event {EventName}", envelope.MessageId, envelope.EventName);
        return Task.CompletedTask;
    }
}

internal sealed class OutboxSerializer : IOutboxSerializer
{
    public OutboxMessage Create<T>(string eventName, MessageEnvelope<T> envelope)
        => OutboxMessage.Create(eventName, JsonSerializer.Serialize(envelope), envelope.CorrelationId, envelope.MessageId, envelope.OccurredAt);
}

public sealed class OutboxDispatcherService(IServiceScopeFactory scopeFactory, ILogger<OutboxDispatcherService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var repository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var publisher = scope.ServiceProvider.GetRequiredService<IMessageBusPublisher>();
                var clock = scope.ServiceProvider.GetRequiredService<IClock>();

                var pendingMessages = await repository.GetPendingAsync(20, stoppingToken);
                foreach (var message in pendingMessages)
                {
                    try
                    {
                        var payloadType = message.EventName switch
                        {
                            "investment-order-created" => typeof(Toro.Testes.Contracts.Events.InvestmentOrderCreatedIntegrationEvent),
                            "investment-order-processed" => typeof(Toro.Testes.Contracts.Events.InvestmentOrderProcessedIntegrationEvent),
                            "investment-order-failed" => typeof(Toro.Testes.Contracts.Events.InvestmentOrderFailedIntegrationEvent),
                            _ => null
                        };

                        if (payloadType is null)
                        {
                            message.MarkFailed();
                            continue;
                        }

                        var envelopeType = typeof(MessageEnvelope<>).MakeGenericType(payloadType);
                        var envelope = JsonSerializer.Deserialize(message.Payload, envelopeType);
                        var publishMethod = typeof(IMessageBusPublisher).GetMethod(nameof(IMessageBusPublisher.PublishAsync))!.MakeGenericMethod(payloadType);
                        await (Task)publishMethod.Invoke(publisher, [envelope!, message.EventName, stoppingToken])!;
                        message.MarkPublished(clock.UtcNow);
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "Failed to dispatch outbox message {OutboxMessageId}", message.Id);
                        message.MarkFailed();
                    }
                }

                if (pendingMessages.Count > 0)
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Outbox dispatcher will retry after transient startup failure.");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
