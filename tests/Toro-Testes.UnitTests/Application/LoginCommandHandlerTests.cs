using FluentAssertions;
using Moq;
using Toro.Testes.Application.Features.Auth.Commands.Login;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.Domain.Entities;

namespace Toro.Testes.UnitTests.Application;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Should_generate_token_for_valid_credentials()
    {
        var customer = Customer.Create("Maria", "investor@torotestes.com", "123", "Investor");
        var repository = new Mock<ICustomerRepository>();
        var tokenService = new Mock<IJwtTokenService>();

        repository.Setup(x => x.GetByEmailAsync(customer.Email, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        tokenService.Setup(x => x.GenerateToken(customer)).Returns("jwt-token");

        var handler = new LoginCommandHandler(repository.Object, tokenService.Object);

        var result = await handler.Handle(new LoginCommand(customer.Email, "Toro@123"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("jwt-token");
    }
}
