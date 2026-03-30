using Microsoft.Extensions.DependencyInjection;
using Toro.Testes.BuildingBlocks.Abstractions;
using Toro.Testes.BuildingBlocks.Helpers;

namespace Toro.Testes.BuildingBlocks.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddToroBuildingBlocks(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<ICorrelationContextAccessor, CorrelationContextAccessor>();
        return services;
    }
}

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
