using Toro.Testes.Domain.Enums;

namespace Toro.Testes.Application.DTOs.Requests;

public sealed record CreateInvestmentProductRequest(
    string Name,
    InvestmentType InvestmentType,
    decimal MinimumAmount,
    decimal AnnualInterestRate,
    ProductLiquidityType LiquidityType,
    RiskLevel RiskLevel,
    DateOnly MaturityDate,
    int? GracePeriodDays,
    bool IsActive);
