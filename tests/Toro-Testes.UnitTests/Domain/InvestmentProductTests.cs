using FluentAssertions;
using Toro.Testes.BuildingBlocks.Exceptions;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Domain.Enums;
using Toro.Testes.Domain.ValueObjects;

namespace Toro.Testes.UnitTests.Domain;

public sealed class InvestmentProductTests
{
    [Fact]
    public void Should_require_grace_period_for_lci()
    {
        var action = () => InvestmentProduct.Create(
            "LCI Test",
            InvestmentType.LCI,
            new Money(5000),
            new Percentage(10),
            ProductLiquidityType.GracePeriod,
            RiskLevel.Low,
            new MaturityDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))),
            null);

        action.Should().Throw<BusinessRuleException>();
    }
}
