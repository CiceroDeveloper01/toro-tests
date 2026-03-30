using MediatR;
using Toro.Testes.Application.DTOs.Extensions;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Results;

namespace Toro.Testes.Application.Features.InvestmentProducts.Queries.GetInvestmentProducts;

public sealed record GetInvestmentProductsQuery : IRequest<Result<GetInvestmentProductsResponse>>;

public sealed class GetInvestmentProductsQueryHandler(IInvestmentProductRepository repository)
    : IRequestHandler<GetInvestmentProductsQuery, Result<GetInvestmentProductsResponse>>
{
    public async Task<Result<GetInvestmentProductsResponse>> Handle(GetInvestmentProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return Result<GetInvestmentProductsResponse>.Success(new GetInvestmentProductsResponse(products.Select(x => x.ToResponse()).ToArray()));
    }
}
