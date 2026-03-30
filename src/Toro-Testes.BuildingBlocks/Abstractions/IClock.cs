namespace Toro.Testes.BuildingBlocks.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
