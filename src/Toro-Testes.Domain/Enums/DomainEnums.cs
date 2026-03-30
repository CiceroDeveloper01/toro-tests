namespace Toro.Testes.Domain.Enums;

public enum InvestmentType
{
    CDB = 1,
    TESOURO = 2,
    LCI = 3,
    LCA = 4
}

public enum OrderStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Rejected = 4,
    Cancelled = 5
}

public enum ProductLiquidityType
{
    Daily = 1,
    OnMaturity = 2,
    GracePeriod = 3
}

public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3
}
