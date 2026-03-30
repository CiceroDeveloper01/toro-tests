using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Domain.Enums;

namespace Toro.Testes.Infrastructure.Data.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).HasMaxLength(140).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(180).IsRequired();
        builder.Property(x => x.DocumentNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Role).HasMaxLength(40).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}

internal sealed class InvestmentProductConfiguration : IEntityTypeConfiguration<InvestmentProduct>
{
    public void Configure(EntityTypeBuilder<InvestmentProduct> builder)
    {
        builder.ToTable("investment_products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.MinimumAmount).HasColumnType("numeric(18,2)");
        builder.Property(x => x.AnnualInterestRate).HasColumnType("numeric(10,4)");
        builder.Property(x => x.InvestmentType).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.LiquidityType).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.RiskLevel).HasConversion<string>().HasMaxLength(20);
    }
}

internal sealed class InvestmentOrderConfiguration : IEntityTypeConfiguration<InvestmentOrder>
{
    public void Configure(EntityTypeBuilder<InvestmentOrder> builder)
    {
        builder.ToTable("investment_orders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.RejectionReason).HasMaxLength(250);
        builder.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.ProductId);
    }
}

internal sealed class PortfolioPositionConfiguration : IEntityTypeConfiguration<PortfolioPosition>
{
    public void Configure(EntityTypeBuilder<PortfolioPosition> builder)
    {
        builder.ToTable("portfolio_positions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.InvestedAmount).HasColumnType("numeric(18,2)");
        builder.Property(x => x.Quantity).HasColumnType("numeric(18,4)");
        builder.Property(x => x.AverageRate).HasColumnType("numeric(10,4)");
        builder.HasIndex(x => new { x.CustomerId, x.ProductId }).IsUnique();
    }
}

internal sealed class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedMessage> builder)
    {
        builder.ToTable("processed_messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ConsumerName).HasMaxLength(120).IsRequired();
        builder.HasIndex(x => new { x.MessageId, x.ConsumerName }).IsUnique();
    }
}

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Payload).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.Status).HasMaxLength(40).IsRequired();
        builder.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => new { x.Status, x.OccurredOn });
    }
}
