using Toro.Testes.BuildingBlocks.Abstractions;
using Toro.Testes.BuildingBlocks.Exceptions;
using Toro.Testes.Domain.Enums;
using Toro.Testes.Domain.ValueObjects;

namespace Toro.Testes.Domain.Entities;

public sealed class InvestmentProduct : AuditableEntity
{
    private InvestmentProduct()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public InvestmentType InvestmentType { get; private set; }
    public decimal MinimumAmount { get; private set; }
    public decimal AnnualInterestRate { get; private set; }
    public ProductLiquidityType LiquidityType { get; private set; }
    public RiskLevel RiskLevel { get; private set; }
    public DateOnly MaturityDate { get; private set; }
    public int? GracePeriodDays { get; private set; }
    public bool IsActive { get; private set; }

    public static InvestmentProduct Create(
        string name,
        InvestmentType investmentType,
        Money minimumAmount,
        Percentage annualInterestRate,
        ProductLiquidityType liquidityType,
        RiskLevel riskLevel,
        MaturityDate maturityDate,
        int? gracePeriodDays,
        bool isActive = true)
    {
        if ((investmentType is InvestmentType.LCI or InvestmentType.LCA) && (!gracePeriodDays.HasValue || gracePeriodDays <= 0))
        {
            throw new BusinessRuleException("LCI and LCA products must define a grace period.");
        }

        return new InvestmentProduct
        {
            Id = Guid.NewGuid(),
            Name = name,
            InvestmentType = investmentType,
            MinimumAmount = minimumAmount.Value,
            AnnualInterestRate = annualInterestRate.Value,
            LiquidityType = liquidityType,
            RiskLevel = riskLevel,
            MaturityDate = maturityDate.Value,
            GracePeriodDays = gracePeriodDays,
            IsActive = isActive
        };
    }

    public void EnsureMinimumAmount(decimal amount)
    {
        if (amount < MinimumAmount)
        {
            throw new BusinessRuleException($"Order amount must be at least {MinimumAmount:N2}.");
        }
    }
}
