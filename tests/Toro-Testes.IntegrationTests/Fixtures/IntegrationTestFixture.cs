using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Toro.Testes.Application.DependencyInjection;
using Toro.Testes.Infrastructure.DependencyInjection;
using Toro.Testes.Infrastructure.Data;
using Toro.Testes.Worker.Consumers;

namespace Toro.Testes.IntegrationTests.Fixtures;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithDatabase("toro_testes")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder()
        .WithUsername("toro")
        .WithPassword("toro")
        .Build();

    private IHost? _workerHost;

    public WebApplicationFactory<Program> Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _rabbitMq.StartAsync();

        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:PostgreSql"] = _postgres.GetConnectionString(),
            ["RabbitMq:HostName"] = _rabbitMq.Hostname,
            ["RabbitMq:Port"] = _rabbitMq.GetMappedPublicPort(5672).ToString(),
            ["RabbitMq:UserName"] = "toro",
            ["RabbitMq:Password"] = "toro",
            ["Jwt:SecretKey"] = "super-secret-key-for-local-development-only-123456"
        };

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(settings));
                builder.ConfigureServices(services =>
                {
                    using var scope = services.BuildServiceProvider().CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Database.EnsureCreated();
                });
            });

        Client = Factory.CreateClient();
        await StartWorkerAsync(settings);
        Client.DefaultRequestHeaders.Add("X-Correlation-Id", Guid.NewGuid().ToString("N"));
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        if (_workerHost is not null)
        {
            await _workerHost.StopAsync();
            _workerHost.Dispose();
        }

        await _rabbitMq.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    public async Task<string> LoginAsInvestorAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", new { email = "investor@torotestes.com", password = "Toro@123" });
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Login failed with {(int)response.StatusCode}: {error}");
        }

        var payload = await response.Content.ReadFromJsonAsync<LoginEnvelope>();
        return payload!.Data.AccessToken;
    }

    private async Task StartWorkerAsync(Dictionary<string, string?> settings)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection(settings);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration, "Toro-Testes.Worker.Tests");
        builder.Services.AddHostedService<InvestmentOrderCreatedConsumer>();

        _workerHost = builder.Build();
        await _workerHost.StartAsync();
    }

    public sealed record LoginEnvelope(LoginData Data);
    public sealed record LoginData(string AccessToken);
}
