using MediatR;
using Toro.Testes.Application.DTOs.Extensions;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Exceptions;
using Toro.Testes.BuildingBlocks.Results;

namespace Toro.Testes.Application.Features.InvestmentOrders.Queries.GetInvestmentOrderById;

public sealed record GetInvestmentOrderByIdQuery(Guid OrderId) : IRequest<Result<GetInvestmentOrderByIdResponse>>;

public sealed class GetInvestmentOrderByIdQueryHandler(IInvestmentOrderRepository repository)
    : IRequestHandler<GetInvestmentOrderByIdQuery, Result<GetInvestmentOrderByIdResponse>>
{
    public async Task<Result<GetInvestmentOrderByIdResponse>> Handle(GetInvestmentOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException("Investment order not found.");

        return Result<GetInvestmentOrderByIdResponse>.Success(order.ToResponse());
    }
}
