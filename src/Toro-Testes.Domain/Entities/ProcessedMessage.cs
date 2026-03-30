using Toro.Testes.BuildingBlocks.Abstractions;

namespace Toro.Testes.Domain.Entities;

public sealed class ProcessedMessage : AuditableEntity
{
    private ProcessedMessage()
    {
    }

    public Guid MessageId { get; private set; }
    public string ConsumerName { get; private set; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; private set; }

    public static ProcessedMessage Create(Guid messageId, string consumerName, DateTimeOffset processedAt)
        => new()
        {
            Id = Guid.NewGuid(),
            MessageId = messageId,
            ConsumerName = consumerName,
            ProcessedAt = processedAt
        };
}
