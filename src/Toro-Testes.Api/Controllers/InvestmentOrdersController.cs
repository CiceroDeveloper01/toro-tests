using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toro.Testes.Api.Common;
using Toro.Testes.Application.DTOs.Extensions;
using Toro.Testes.Application.DTOs.Requests;
using Toro.Testes.Application.Features.InvestmentOrders.Queries.GetInvestmentOrderById;
using Toro.Testes.Application.Features.InvestmentOrders.Queries.GetInvestmentOrdersByCustomer;
using Toro.Testes.BuildingBlocks.Constants;
using Toro.Testes.BuildingBlocks.Helpers;

namespace Toro.Testes.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/investment-orders")]
[Authorize(Policy = ApplicationConstants.Policies.InvestorAccess)]
public sealed class InvestmentOrdersController(IMediator mediator, ICorrelationContextAccessor correlationContextAccessor) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Create([FromBody] CreateInvestmentOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(correlationContextAccessor.CorrelationId), cancellationToken);
        return Accepted(new ApiResponse<object>(result.Message, result.Value!, correlationContextAccessor.CorrelationId));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetInvestmentOrderByIdQuery(id), cancellationToken);
        return Ok(new ApiResponse<object>(result.Message, result.Value!, correlationContextAccessor.CorrelationId));
    }

    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetInvestmentOrdersByCustomerQuery(customerId), cancellationToken);
        return Ok(new ApiResponse<object>(result.Message, result.Value!, correlationContextAccessor.CorrelationId));
    }
}
