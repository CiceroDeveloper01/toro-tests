using MediatR;
using Toro.Testes.Application.DTOs.Extensions;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Exceptions;
using Toro.Testes.BuildingBlocks.Results;

namespace Toro.Testes.Application.Features.InvestmentProducts.Queries.GetInvestmentProductById;

public sealed record GetInvestmentProductByIdQuery(Guid ProductId) : IRequest<Result<GetInvestmentProductResponse>>;

public sealed class GetInvestmentProductByIdQueryHandler(IInvestmentProductRepository repository)
    : IRequestHandler<GetInvestmentProductByIdQuery, Result<GetInvestmentProductResponse>>
{
    public async Task<Result<GetInvestmentProductResponse>> Handle(GetInvestmentProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Investment product not found.");

        return Result<GetInvestmentProductResponse>.Success(product.ToResponse());
    }
}
