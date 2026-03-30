namespace Toro.Testes.BuildingBlocks.Helpers;

public interface ICorrelationContextAccessor
{
    string CorrelationId { get; set; }
}

public sealed class CorrelationContextAccessor : ICorrelationContextAccessor
{
    private string _correlationId = Guid.NewGuid().ToString("N");

    public string CorrelationId
    {
        get => _correlationId;
        set => _correlationId = string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("N") : value;
    }
}
