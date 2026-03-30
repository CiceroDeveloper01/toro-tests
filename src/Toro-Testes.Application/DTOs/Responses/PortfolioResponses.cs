namespace Toro.Testes.Application.DTOs.Responses;

public sealed record PortfolioPositionResponse(
    Guid Id,
    Guid CustomerId,
    Guid ProductId,
    decimal InvestedAmount,
    decimal Quantity,
    decimal AverageRate,
    DateTimeOffset? UpdatedAt);

public sealed record GetPortfolioResponse(Guid CustomerId, decimal TotalInvestedAmount, int PositionsCount);

public sealed record GetPortfolioPositionsResponse(IReadOnlyCollection<PortfolioPositionResponse> Positions);
