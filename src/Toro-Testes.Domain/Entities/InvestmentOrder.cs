using Toro.Testes.BuildingBlocks.Abstractions;
using Toro.Testes.BuildingBlocks.Exceptions;
using Toro.Testes.Domain.Enums;
using Toro.Testes.Domain.Events;
using Toro.Testes.Domain.ValueObjects;

namespace Toro.Testes.Domain.Entities;

public sealed class InvestmentOrder : AuditableEntity
{
    private InvestmentOrder()
    {
    }

    public Guid CustomerId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Amount { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public string CorrelationId { get; private set; } = string.Empty;

    public static InvestmentOrder Create(Guid customerId, Guid productId, Money amount, string correlationId)
    {
        var order = new InvestmentOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductId = productId,
            Amount = amount.Value,
            Status = OrderStatus.Pending,
            CorrelationId = correlationId
        };

        order.AddDomainEvent(new InvestmentOrderCreatedDomainEvent(order.Id, customerId, productId, amount.Value, correlationId));
        return order;
    }

    public void StartProcessing()
    {
        EnsureCanBeProcessed();
        Status = OrderStatus.Processing;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        if (Status is OrderStatus.Completed or OrderStatus.Rejected or OrderStatus.Cancelled)
        {
            throw new BusinessRuleException("Order can no longer be completed.");
        }

        Status = OrderStatus.Completed;
        UpdatedAt = DateTimeOffset.UtcNow;
        RejectionReason = null;
    }

    public void Reject(string reason)
    {
        if (Status is OrderStatus.Completed or OrderStatus.Cancelled)
        {
            throw new BusinessRuleException("Order can no longer be rejected.");
        }

        Status = OrderStatus.Rejected;
        UpdatedAt = DateTimeOffset.UtcNow;
        RejectionReason = reason;
    }

    public void EnsureCanBeProcessed()
    {
        if (Status is not OrderStatus.Pending)
        {
            throw new BusinessRuleException("Order cannot be processed twice.");
        }
    }
}
