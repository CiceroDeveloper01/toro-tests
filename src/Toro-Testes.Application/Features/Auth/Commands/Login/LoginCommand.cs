using FluentValidation;
using MediatR;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Exceptions;
using Toro.Testes.BuildingBlocks.Results;

namespace Toro.Testes.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(4);
    }
}

public sealed class LoginCommandHandler(ICustomerRepository customerRepository, IJwtTokenService jwtTokenService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedException("Invalid credentials.");

        // Simple local authentication for demo/portfolio purposes.
        if (!string.Equals(request.Password, "Toro@123", StringComparison.Ordinal))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var token = jwtTokenService.GenerateToken(customer);
        var response = new LoginResponse(token, DateTimeOffset.UtcNow.AddHours(2), customer.Id, customer.Role);
        return Result<LoginResponse>.Success(response, "Token generated successfully.");
    }
}
