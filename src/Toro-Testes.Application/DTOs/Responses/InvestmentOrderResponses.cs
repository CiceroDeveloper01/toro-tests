using Toro.Testes.Domain.Enums;

namespace Toro.Testes.Application.DTOs.Responses;

public sealed record CreateInvestmentOrderResponse(Guid OrderId, OrderStatus Status, string Message, string CorrelationId);

public sealed record GetInvestmentOrderByIdResponse(
    Guid Id,
    Guid CustomerId,
    Guid ProductId,
    decimal Amount,
    OrderStatus Status,
    string? RejectionReason,
    string CorrelationId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public sealed record GetInvestmentOrdersByCustomerResponse(IReadOnlyCollection<GetInvestmentOrderByIdResponse> Orders);
