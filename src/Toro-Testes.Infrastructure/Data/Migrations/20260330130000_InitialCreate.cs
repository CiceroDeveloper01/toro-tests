using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toro.Testes.Infrastructure.Data.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "customers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                FullName = table.Column<string>(type: "character varying(140)", maxLength: 140, nullable: false),
                Email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                DocumentNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Role = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_customers", x => x.Id));

        migrationBuilder.CreateTable(
            name: "investment_orders",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                RejectionReason = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_investment_orders", x => x.Id));

        migrationBuilder.CreateTable(
            name: "investment_products",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                InvestmentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                MinimumAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                AnnualInterestRate = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                LiquidityType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                RiskLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                MaturityDate = table.Column<DateOnly>(type: "date", nullable: false),
                GracePeriodDays = table.Column<int>(type: "integer", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_investment_products", x => x.Id));

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                EventName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Payload = table.Column<string>(type: "jsonb", nullable: false),
                OccurredOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                PublishedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_outbox_messages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "portfolio_positions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                InvestedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                Quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                AverageRate = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_portfolio_positions", x => x.Id));

        migrationBuilder.CreateTable(
            name: "processed_messages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                ConsumerName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_processed_messages", x => x.Id));

        migrationBuilder.InsertData(
            table: "customers",
            columns: new[] { "Id", "CreatedAt", "DocumentNumber", "Email", "FullName", "Role", "UpdatedAt" },
            values: new object[,]
            {
                { Seeder.AdminId, DateTimeOffset.UtcNow, "00000000000", "admin@torotestes.com", "Toro Admin", "Admin", null },
                { Seeder.InvestorId, DateTimeOffset.UtcNow, "12345678900", "investor@torotestes.com", "Maria Investidora", "Investor", null }
            });

        migrationBuilder.InsertData(
            table: "investment_products",
            columns: new[] { "Id", "AnnualInterestRate", "CreatedAt", "GracePeriodDays", "InvestmentType", "IsActive", "LiquidityType", "MaturityDate", "MinimumAmount", "Name", "RiskLevel", "UpdatedAt" },
            values: new object[,]
            {
                { Seeder.CdbId, 12.5m, DateTimeOffset.UtcNow, null, "CDB", true, "Daily", DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(730)), 1000m, "CDB Banco Alfa 110% CDI", "Low", null },
                { Seeder.TesouroId, 10.1m, DateTimeOffset.UtcNow, null, "TESOURO", true, "OnMaturity", DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1100)), 100m, "Tesouro Prefixado 2029", "Low", null },
                { Seeder.LciId, 9.8m, DateTimeOffset.UtcNow, 90, "LCI", true, "GracePeriod", DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(900)), 5000m, "LCI Premium 95% CDI", "Low", null },
                { Seeder.LcaId, 9.9m, DateTimeOffset.UtcNow, 120, "LCA", true, "GracePeriod", DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1080)), 5000m, "LCA Agro Plus 96% CDI", "Medium", null }
            });

        migrationBuilder.CreateIndex(name: "IX_customers_Email", table: "customers", column: "Email", unique: true);
        migrationBuilder.CreateIndex(name: "IX_investment_orders_CustomerId", table: "investment_orders", column: "CustomerId");
        migrationBuilder.CreateIndex(name: "IX_investment_orders_ProductId", table: "investment_orders", column: "ProductId");
        migrationBuilder.CreateIndex(name: "IX_outbox_messages_Status_OccurredOn", table: "outbox_messages", columns: new[] { "Status", "OccurredOn" });
        migrationBuilder.CreateIndex(name: "IX_portfolio_positions_CustomerId_ProductId", table: "portfolio_positions", columns: new[] { "CustomerId", "ProductId" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_processed_messages_MessageId_ConsumerName", table: "processed_messages", columns: new[] { "MessageId", "ConsumerName" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "customers");
        migrationBuilder.DropTable(name: "investment_orders");
        migrationBuilder.DropTable(name: "investment_products");
        migrationBuilder.DropTable(name: "outbox_messages");
        migrationBuilder.DropTable(name: "portfolio_positions");
        migrationBuilder.DropTable(name: "processed_messages");
    }
}
