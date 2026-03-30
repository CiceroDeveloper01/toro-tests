using Microsoft.EntityFrameworkCore;
using Toro.Testes.Domain.Entities;

namespace Toro.Testes.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<InvestmentProduct> InvestmentProducts => Set<InvestmentProduct>();
    public DbSet<InvestmentOrder> InvestmentOrders => Set<InvestmentOrder>();
    public DbSet<PortfolioPosition> PortfolioPositions => Set<PortfolioPosition>();
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        Seeder.Seed(modelBuilder);
    }
}
