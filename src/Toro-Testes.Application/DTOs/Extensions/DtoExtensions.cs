using Toro.Testes.Application.DTOs.Requests;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Features.Auth.Commands.Login;
using Toro.Testes.Application.Features.InvestmentOrders.Commands.CreateInvestmentOrder;
using Toro.Testes.Application.Features.InvestmentProducts.Commands.CreateInvestmentProduct;
using Toro.Testes.Domain.Entities;

namespace Toro.Testes.Application.DTOs.Extensions;

public static class AuthDtoExtension
{
    public static LoginCommand ToCommand(this LoginRequest request) => new(request.Email, request.Password);
}

public static class InvestmentProductDtoExtension
{
    public static CreateInvestmentProductCommand ToCommand(this CreateInvestmentProductRequest request) =>
        new(
            request.Name,
            request.InvestmentType,
            request.MinimumAmount,
            request.AnnualInterestRate,
            request.LiquidityType,
            request.RiskLevel,
            request.MaturityDate,
            request.GracePeriodDays,
            request.IsActive);

    public static GetInvestmentProductResponse ToResponse(this InvestmentProduct product) =>
        new(
            product.Id,
            product.Name,
            product.InvestmentType,
            product.MinimumAmount,
            product.AnnualInterestRate,
            product.LiquidityType,
            product.RiskLevel,
            product.MaturityDate,
            product.GracePeriodDays,
            product.IsActive);
}

public static class InvestmentOrderDtoExtension
{
    public static CreateInvestmentOrderCommand ToCommand(this CreateInvestmentOrderRequest request, string correlationId) =>
        new(request.CustomerId, request.ProductId, request.Amount, correlationId);

    public static GetInvestmentOrderByIdResponse ToResponse(this InvestmentOrder order) =>
        new(
            order.Id,
            order.CustomerId,
            order.ProductId,
            order.Amount,
            order.Status,
            order.RejectionReason,
            order.CorrelationId,
            order.CreatedAt,
            order.UpdatedAt);
}

public static class PortfolioDtoExtension
{
    public static PortfolioPositionResponse ToResponse(this PortfolioPosition position) =>
        new(
            position.Id,
            position.CustomerId,
            position.ProductId,
            position.InvestedAmount,
            position.Quantity,
            position.AverageRate,
            position.UpdatedAt);
}
