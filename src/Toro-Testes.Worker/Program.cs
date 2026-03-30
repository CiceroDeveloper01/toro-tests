using Serilog;
using Toro.Testes.Application.DependencyInjection;
using Toro.Testes.Infrastructure.DependencyInjection;
using Toro.Testes.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, "Toro-Testes.Worker", includeAuth: false);
builder.Services.AddHostedService<InvestmentOrderCreatedConsumer>();
builder.Logging.AddSerilog(Log.Logger);

var host = builder.Build();

await host.RunAsync();
