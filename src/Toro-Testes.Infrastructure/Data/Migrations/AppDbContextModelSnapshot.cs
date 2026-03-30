using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Toro.Testes.Infrastructure.Data;

#nullable disable

namespace Toro.Testes.Infrastructure.Data.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.14")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("Toro.Testes.Domain.Entities.Customer", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
            b.Property<string>("DocumentNumber").IsRequired().HasMaxLength(20).HasColumnType("character varying(20)");
            b.Property<string>("Email").IsRequired().HasMaxLength(180).HasColumnType("character varying(180)");
            b.Property<string>("FullName").IsRequired().HasMaxLength(140).HasColumnType("character varying(140)");
            b.Property<string>("Role").IsRequired().HasMaxLength(40).HasColumnType("character varying(40)");
            b.Property<DateTimeOffset?>("UpdatedAt").HasColumnType("timestamp with time zone");
            b.HasKey("Id");
            b.HasIndex("Email").IsUnique();
            b.ToTable("customers", (string)null);
        });

        modelBuilder.Entity("Toro.Testes.Domain.Entities.InvestmentOrder", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            b.Property<decimal>("Amount").HasColumnType("numeric(18,2)");
            b.Property<string>("CorrelationId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
            b.Property<Guid>("CustomerId").HasColumnType("uuid");
            b.Property<Guid>("ProductId").HasColumnType("uuid");
            b.Property<string>("RejectionReason").HasMaxLength(250).HasColumnType("character varying(250)");
            b.Property<string>("Status").IsRequired().HasMaxLength(20).HasColumnType("character varying(20)");
            b.Property<DateTimeOffset?>("UpdatedAt").HasColumnType("timestamp with time zone");
            b.HasKey("Id");
            b.HasIndex("CustomerId");
            b.HasIndex("ProductId");
            b.ToTable("investment_orders", (string)null);
        });

        modelBuilder.Entity("Toro.Testes.Domain.Entities.InvestmentProduct", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            b.Property<decimal>("AnnualInterestRate").HasColumnType("numeric(10,4)");
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
            b.Property<int?>("GracePeriodDays").HasColumnType("integer");
            b.Property<string>("InvestmentType").IsRequired().HasMaxLength(20).HasColumnType("character varying(20)");
            b.Property<bool>("IsActive").HasColumnType("boolean");
            b.Property<string>("LiquidityType").IsRequired().HasMaxLength(20).HasColumnType("character varying(20)");
            b.Property<DateOnly>("MaturityDate").HasColumnType("date");
            b.Property<decimal>("MinimumAmount").HasColumnType("numeric(18,2)");
            b.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("character varying(120)");
            b.Property<string>("RiskLevel").IsRequired().HasMaxLength(20).HasColumnType("character varying(20)");
            b.Property<DateTimeOffset?>("UpdatedAt").HasColumnType("timestamp with time zone");
            b.HasKey("Id");
            b.ToTable("investment_products", (string)null);
        });

        modelBuilder.Entity("Toro.Testes.Domain.Entities.OutboxMessage", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            b.Property<string>("CorrelationId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
            b.Property<string>("EventName").IsRequired().HasMaxLength(120).HasColumnType("character varying(120)");
            b.Property<Guid>("MessageId").HasColumnType("uuid");
            b.Property<DateTimeOffset>("OccurredOn").HasColumnType("timestamp with time zone");
            b.Property<string>("Payload").IsRequired().HasColumnType("jsonb");
            b.Property<DateTimeOffset?>("PublishedOn").HasColumnType("timestamp with time zone");
            b.Property<string>("Status").IsRequired().HasMaxLength(40).HasColumnType("character varying(40)");
            b.Property<DateTimeOffset?>("UpdatedAt").HasColumnType("timestamp with time zone");
            b.HasKey("Id");
            b.HasIndex("Status", "OccurredOn");
            b.ToTable("outbox_messages", (string)null);
        });

        modelBuilder.Entity("Toro.Testes.Domain.Entities.PortfolioPosition", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            b.Property<decimal>("AverageRate").HasColumnType("numeric(10,4)");
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
            b.Property<Guid>("CustomerId").HasColumnType("uuid");
            b.Property<decimal>("InvestedAmount").HasColumnType("numeric(18,2)");
            b.Property<Guid>("ProductId").HasColumnType("uuid");
            b.Property<decimal>("Quantity").HasColumnType("numeric(18,4)");
            b.Property<DateTimeOffset?>("UpdatedAt").HasColumnType("timestamp with time zone");
            b.HasKey("Id");
            b.HasIndex("CustomerId", "ProductId").IsUnique();
            b.ToTable("portfolio_positions", (string)null);
        });

        modelBuilder.Entity("Toro.Testes.Domain.Entities.ProcessedMessage", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            b.Property<string>("ConsumerName").IsRequired().HasMaxLength(120).HasColumnType("character varying(120)");
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
            b.Property<Guid>("MessageId").HasColumnType("uuid");
            b.Property<DateTimeOffset>("ProcessedAt").HasColumnType("timestamp with time zone");
            b.Property<DateTimeOffset?>("UpdatedAt").HasColumnType("timestamp with time zone");
            b.HasKey("Id");
            b.HasIndex("MessageId", "ConsumerName").IsUnique();
            b.ToTable("processed_messages", (string)null);
        });
    }
}
