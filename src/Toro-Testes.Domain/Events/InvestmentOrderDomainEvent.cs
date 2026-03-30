namespace Toro.Testes.Domain.Events;

public sealed record InvestmentOrderCreatedDomainEvent(Guid OrderId, Guid CustomerId, Guid ProductId, decimal Amount, string CorrelationId);
