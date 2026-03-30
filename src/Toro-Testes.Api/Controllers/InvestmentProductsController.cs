using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toro.Testes.Api.Common;
using Toro.Testes.Application.DTOs.Extensions;
using Toro.Testes.Application.DTOs.Requests;
using Toro.Testes.Application.Features.InvestmentProducts.Queries.GetInvestmentProductById;
using Toro.Testes.Application.Features.InvestmentProducts.Queries.GetInvestmentProducts;
using Toro.Testes.BuildingBlocks.Constants;
using Toro.Testes.BuildingBlocks.Helpers;

namespace Toro.Testes.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/investment-products")]
[Authorize(Policy = ApplicationConstants.Policies.InvestorAccess)]
public sealed class InvestmentProductsController(IMediator mediator, ICorrelationContextAccessor correlationContextAccessor) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetInvestmentProductsQuery(), cancellationToken);
        return Ok(new ApiResponse<object>(result.Message, result.Value!, correlationContextAccessor.CorrelationId));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetInvestmentProductByIdQuery(id), cancellationToken);
        return Ok(new ApiResponse<object>(result.Message, result.Value!, correlationContextAccessor.CorrelationId));
    }

    [HttpPost]
    [Authorize(Policy = ApplicationConstants.Policies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateInvestmentProductRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.ProductId, version = "1" }, new ApiResponse<object>(result.Message, result.Value!, correlationContextAccessor.CorrelationId));
    }
}
