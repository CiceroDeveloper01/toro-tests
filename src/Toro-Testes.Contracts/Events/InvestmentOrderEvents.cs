namespace Toro.Testes.Contracts.Events;

public sealed record InvestmentOrderCreatedIntegrationEvent(
    Guid OrderId,
    Guid CustomerId,
    Guid ProductId,
    decimal Amount,
    string Status);

public sealed record InvestmentOrderProcessedIntegrationEvent(
    Guid OrderId,
    Guid CustomerId,
    Guid ProductId,
    decimal Amount,
    string Status,
    DateTimeOffset ProcessedAt);

public sealed record InvestmentOrderFailedIntegrationEvent(
    Guid OrderId,
    Guid CustomerId,
    Guid ProductId,
    decimal Amount,
    string Status,
    string Reason,
    DateTimeOffset FailedAt);
