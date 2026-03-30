using MediatR;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Results;

namespace Toro.Testes.Application.Features.Portfolio.Queries.GetPortfolio;

public sealed record GetPortfolioQuery(Guid CustomerId) : IRequest<Result<GetPortfolioResponse>>;

public sealed class GetPortfolioQueryHandler(IPortfolioPositionRepository repository)
    : IRequestHandler<GetPortfolioQuery, Result<GetPortfolioResponse>>
{
    public async Task<Result<GetPortfolioResponse>> Handle(GetPortfolioQuery request, CancellationToken cancellationToken)
    {
        var positions = await repository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return Result<GetPortfolioResponse>.Success(new GetPortfolioResponse(request.CustomerId, positions.Sum(x => x.InvestedAmount), positions.Count));
    }
}
