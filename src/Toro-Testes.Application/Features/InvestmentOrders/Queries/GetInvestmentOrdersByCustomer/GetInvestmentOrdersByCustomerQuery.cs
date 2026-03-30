using MediatR;
using Toro.Testes.Application.DTOs.Extensions;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Results;

namespace Toro.Testes.Application.Features.InvestmentOrders.Queries.GetInvestmentOrdersByCustomer;

public sealed record GetInvestmentOrdersByCustomerQuery(Guid CustomerId) : IRequest<Result<GetInvestmentOrdersByCustomerResponse>>;

public sealed class GetInvestmentOrdersByCustomerQueryHandler(IInvestmentOrderRepository repository)
    : IRequestHandler<GetInvestmentOrdersByCustomerQuery, Result<GetInvestmentOrdersByCustomerResponse>>
{
    public async Task<Result<GetInvestmentOrdersByCustomerResponse>> Handle(GetInvestmentOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var orders = await repository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return Result<GetInvestmentOrdersByCustomerResponse>.Success(new GetInvestmentOrdersByCustomerResponse(orders.Select(x => x.ToResponse()).ToArray()));
    }
}
