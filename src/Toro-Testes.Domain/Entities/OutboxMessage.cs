using Toro.Testes.BuildingBlocks.Abstractions;

namespace Toro.Testes.Domain.Entities;

public sealed class OutboxMessage : AuditableEntity
{
    private OutboxMessage()
    {
    }

    public string EventName { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset OccurredOn { get; private set; }
    public DateTimeOffset? PublishedOn { get; private set; }
    public string Status { get; private set; } = "Pending";
    public string CorrelationId { get; private set; } = string.Empty;
    public Guid MessageId { get; private set; }

    public static OutboxMessage Create(string eventName, string payload, string correlationId, Guid messageId, DateTimeOffset occurredOn)
        => new()
        {
            Id = Guid.NewGuid(),
            EventName = eventName,
            Payload = payload,
            CorrelationId = correlationId,
            MessageId = messageId,
            OccurredOn = occurredOn
        };

    public void MarkPublished(DateTimeOffset publishedOn)
    {
        PublishedOn = publishedOn;
        Status = "Published";
        UpdatedAt = publishedOn;
    }

    public void MarkFailed()
    {
        Status = "Failed";
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
