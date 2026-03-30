using MediatR;
using Toro.Testes.Application.DTOs.Extensions;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Results;

namespace Toro.Testes.Application.Features.Portfolio.Queries.GetPortfolioPositions;

public sealed record GetPortfolioPositionsQuery(Guid CustomerId) : IRequest<Result<GetPortfolioPositionsResponse>>;

public sealed class GetPortfolioPositionsQueryHandler(IPortfolioPositionRepository repository)
    : IRequestHandler<GetPortfolioPositionsQuery, Result<GetPortfolioPositionsResponse>>
{
    public async Task<Result<GetPortfolioPositionsResponse>> Handle(GetPortfolioPositionsQuery request, CancellationToken cancellationToken)
    {
        var positions = await repository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return Result<GetPortfolioPositionsResponse>.Success(new GetPortfolioPositionsResponse(positions.Select(x => x.ToResponse()).ToArray()));
    }
}
