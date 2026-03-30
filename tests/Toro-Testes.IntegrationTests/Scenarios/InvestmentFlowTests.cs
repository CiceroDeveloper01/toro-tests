using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Toro.Testes.IntegrationTests.Fixtures;

namespace Toro.Testes.IntegrationTests.Scenarios;

public sealed class InvestmentFlowTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    [Fact]
    public async Task Should_complete_end_to_end_order_flow()
    {
        var token = await fixture.LoginAsInvestorAsync();
        fixture.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productsResponse = await fixture.Client.GetAsync("/api/v1/investment-products");
        productsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPayload = JsonDocument.Parse(await productsResponse.Content.ReadAsStringAsync());
        var productId = productsPayload.RootElement.GetProperty("data").GetProperty("products")[0].GetProperty("id").GetGuid();
        var customerId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var createOrderResponse = await fixture.Client.PostAsJsonAsync("/api/v1/investment-orders", new
        {
            customerId,
            productId,
            amount = 6000m
        });

        createOrderResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var orderPayload = JsonDocument.Parse(await createOrderResponse.Content.ReadAsStringAsync());
        var orderId = orderPayload.RootElement.GetProperty("data").GetProperty("orderId").GetGuid();

        var completed = false;
        for (var attempt = 0; attempt < 20 && !completed; attempt++)
        {
            await Task.Delay(1000);
            var orderResponse = await fixture.Client.GetAsync($"/api/v1/investment-orders/{orderId}");
            var orderDocument = JsonDocument.Parse(await orderResponse.Content.ReadAsStringAsync());
            var status = orderDocument.RootElement.GetProperty("data").GetProperty("status").GetString();
            completed = status is "Completed" or "Rejected";
        }

        completed.Should().BeTrue();

        var portfolioResponse = await fixture.Client.GetAsync($"/api/v1/portfolio/{customerId}/positions");
        portfolioResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
