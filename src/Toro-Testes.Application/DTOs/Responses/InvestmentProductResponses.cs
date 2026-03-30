using Toro.Testes.Domain.Enums;

namespace Toro.Testes.Application.DTOs.Responses;

public sealed record CreateInvestmentProductResponse(Guid ProductId, string Message);

public sealed record GetInvestmentProductResponse(
    Guid Id,
    string Name,
    InvestmentType InvestmentType,
    decimal MinimumAmount,
    decimal AnnualInterestRate,
    ProductLiquidityType LiquidityType,
    RiskLevel RiskLevel,
    DateOnly MaturityDate,
    int? GracePeriodDays,
    bool IsActive);

public sealed record GetInvestmentProductsResponse(IReadOnlyCollection<GetInvestmentProductResponse> Products);
