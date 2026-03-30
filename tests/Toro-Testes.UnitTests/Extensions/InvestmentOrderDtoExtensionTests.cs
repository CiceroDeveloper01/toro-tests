using FluentAssertions;
using Toro.Testes.Application.DTOs.Extensions;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Domain.ValueObjects;

namespace Toro.Testes.UnitTests.Extensions;

public sealed class InvestmentOrderDtoExtensionTests
{
    [Fact]
    public void Should_map_order_entity_to_response()
    {
        var order = InvestmentOrder.Create(Guid.NewGuid(), Guid.NewGuid(), new Money(2500), "corr-map");

        var response = order.ToResponse();

        response.Id.Should().Be(order.Id);
        response.Status.Should().Be(order.Status);
        response.CorrelationId.Should().Be("corr-map");
    }
}
