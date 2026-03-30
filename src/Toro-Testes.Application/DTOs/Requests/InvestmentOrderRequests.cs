namespace Toro.Testes.Application.DTOs.Requests;

public sealed record CreateInvestmentOrderRequest(Guid CustomerId, Guid ProductId, decimal Amount);
