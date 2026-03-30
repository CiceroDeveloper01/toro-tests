using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toro.Testes.Api.Common;
using Toro.Testes.Application.Features.Portfolio.Queries.GetPortfolio;
using Toro.Testes.Application.Features.Portfolio.Queries.GetPortfolioPositions;
using Toro.Testes.BuildingBlocks.Constants;
using Toro.Testes.BuildingBlocks.Helpers;

namespace Toro.Testes.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/portfolio")]
[Authorize(Policy = ApplicationConstants.Policies.InvestorAccess)]
public sealed class PortfolioController(IMediator mediator, ICorrelationContextAccessor correlationContextAccessor) : ControllerBase
{
    [HttpGet("{customerId:guid}")]
    public async Task<IActionResult> GetPortfolio(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPortfolioQuery(customerId), cancellationToken);
        return Ok(new ApiResponse<object>(result.Message, result.Value!, correlationContextAccessor.CorrelationId));
    }

    [HttpGet("{customerId:guid}/positions")]
    public async Task<IActionResult> GetPositions(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPortfolioPositionsQuery(customerId), cancellationToken);
        return Ok(new ApiResponse<object>(result.Message, result.Value!, correlationContextAccessor.CorrelationId));
    }
}
