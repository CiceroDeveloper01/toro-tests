namespace Toro.Testes.Contracts.Common;

public sealed class MessageEnvelope<TPayload>
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public string CorrelationId { get; init; } = Guid.NewGuid().ToString("N");
    public string EventName { get; init; } = string.Empty;
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public TPayload Payload { get; init; } = default!;
}
