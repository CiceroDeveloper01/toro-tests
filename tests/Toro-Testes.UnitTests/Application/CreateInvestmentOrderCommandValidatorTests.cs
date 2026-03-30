using FluentAssertions;
using Toro.Testes.Application.Features.InvestmentOrders.Commands.CreateInvestmentOrder;

namespace Toro.Testes.UnitTests.Application;

public sealed class CreateInvestmentOrderCommandValidatorTests
{
    private readonly CreateInvestmentOrderCommandValidator _validator = new();

    [Fact]
    public void Should_fail_when_amount_is_invalid()
    {
        var command = new CreateInvestmentOrderCommand(Guid.NewGuid(), Guid.NewGuid(), 0, "corr-1");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateInvestmentOrderCommand.Amount));
    }
}
