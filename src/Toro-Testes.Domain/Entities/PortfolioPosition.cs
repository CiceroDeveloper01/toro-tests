using Toro.Testes.BuildingBlocks.Abstractions;

namespace Toro.Testes.Domain.Entities;

public sealed class PortfolioPosition : AuditableEntity
{
    private PortfolioPosition()
    {
    }

    public Guid CustomerId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal InvestedAmount { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal AverageRate { get; private set; }

    public static PortfolioPosition Create(Guid customerId, Guid productId, decimal investedAmount, decimal quantity, decimal averageRate)
        => new()
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductId = productId,
            InvestedAmount = investedAmount,
            Quantity = quantity,
            AverageRate = averageRate
        };

    public void AddInvestment(decimal amount, decimal rate)
    {
        var totalAmount = InvestedAmount + amount;
        AverageRate = totalAmount == 0 ? rate : ((InvestedAmount * AverageRate) + (amount * rate)) / totalAmount;
        InvestedAmount = totalAmount;
        Quantity += 1;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
