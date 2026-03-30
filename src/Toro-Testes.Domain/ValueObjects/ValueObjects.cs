using Toro.Testes.BuildingBlocks.Exceptions;

namespace Toro.Testes.Domain.ValueObjects;

public readonly record struct Money
{
    public Money(decimal value)
    {
        if (value <= 0)
        {
            throw new BusinessRuleException("Amount must be greater than zero.");
        }

        Value = decimal.Round(value, 2, MidpointRounding.ToEven);
    }

    public decimal Value { get; }

    public static implicit operator decimal(Money money) => money.Value;
}

public readonly record struct Percentage
{
    public Percentage(decimal value)
    {
        if (value <= 0 || value > 100)
        {
            throw new BusinessRuleException("Percentage must be between 0 and 100.");
        }

        Value = decimal.Round(value, 4, MidpointRounding.ToEven);
    }

    public decimal Value { get; }

    public static implicit operator decimal(Percentage percentage) => percentage.Value;
}

public readonly record struct MaturityDate
{
    public MaturityDate(DateOnly value)
    {
        if (value <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BusinessRuleException("Maturity date must be in the future.");
        }

        Value = value;
    }

    public DateOnly Value { get; }

    public static implicit operator DateOnly(MaturityDate date) => date.Value;
}
