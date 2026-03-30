using Microsoft.EntityFrameworkCore;
using Toro.Testes.BuildingBlocks.Constants;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Domain.Enums;

namespace Toro.Testes.Infrastructure.Data;

internal static class Seeder
{
    public static readonly Guid AdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid InvestorId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid CdbId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid TesouroId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid LciId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid LcaId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasData(
            new
            {
                Id = AdminId,
                FullName = "Toro Admin",
                Email = "admin@torotestes.com",
                DocumentNumber = "00000000000",
                Role = ApplicationConstants.Roles.Admin,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = (DateTimeOffset?)null
            },
            new
            {
                Id = InvestorId,
                FullName = "Maria Investidora",
                Email = "investor@torotestes.com",
                DocumentNumber = "12345678900",
                Role = ApplicationConstants.Roles.Investor,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = (DateTimeOffset?)null
            });

        modelBuilder.Entity<InvestmentProduct>().HasData(
            CreateProduct(CdbId, "CDB Banco Alfa 110% CDI", InvestmentType.CDB, 1000, 12.5m, ProductLiquidityType.Daily, RiskLevel.Low, 730, null),
            CreateProduct(TesouroId, "Tesouro Prefixado 2029", InvestmentType.TESOURO, 100, 10.1m, ProductLiquidityType.OnMaturity, RiskLevel.Low, 1100, null),
            CreateProduct(LciId, "LCI Premium 95% CDI", InvestmentType.LCI, 5000, 9.8m, ProductLiquidityType.GracePeriod, RiskLevel.Low, 900, 90),
            CreateProduct(LcaId, "LCA Agro Plus 96% CDI", InvestmentType.LCA, 5000, 9.9m, ProductLiquidityType.GracePeriod, RiskLevel.Medium, 1080, 120));
    }

    private static object CreateProduct(
        Guid id,
        string name,
        InvestmentType type,
        decimal minimumAmount,
        decimal annualRate,
        ProductLiquidityType liquidityType,
        RiskLevel riskLevel,
        int maturityDays,
        int? gracePeriodDays)
        => new
        {
            Id = id,
            Name = name,
            InvestmentType = type,
            MinimumAmount = minimumAmount,
            AnnualInterestRate = annualRate,
            LiquidityType = liquidityType,
            RiskLevel = riskLevel,
            MaturityDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(maturityDays)),
            GracePeriodDays = gracePeriodDays,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = (DateTimeOffset?)null
        };
}
