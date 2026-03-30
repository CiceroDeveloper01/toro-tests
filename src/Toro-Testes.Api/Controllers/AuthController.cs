using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Toro.Testes.Api.Common;
using Toro.Testes.Application.DTOs.Extensions;
using Toro.Testes.Application.DTOs.Requests;
using Toro.Testes.BuildingBlocks.Helpers;

namespace Toro.Testes.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController(IMediator mediator, ICorrelationContextAccessor correlationContextAccessor) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        return Ok(new ApiResponse<object>(result.Message, result.Value!, correlationContextAccessor.CorrelationId));
    }
}
