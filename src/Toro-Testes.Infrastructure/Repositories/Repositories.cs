using Microsoft.EntityFrameworkCore;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Infrastructure.Data;

namespace Toro.Testes.Infrastructure.Repositories;

internal sealed class CustomerRepository(AppDbContext dbContext) : ICustomerRepository
{
    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => dbContext.Customers.FirstOrDefaultAsync(x => x.Email == email.ToLower(), cancellationToken);

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}

internal sealed class InvestmentProductRepository(AppDbContext dbContext) : IInvestmentProductRepository
{
    public async Task<IReadOnlyCollection<InvestmentProduct>> GetAllAsync(CancellationToken cancellationToken)
        => await dbContext.InvestmentProducts.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public Task<InvestmentProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.InvestmentProducts.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public Task AddAsync(InvestmentProduct product, CancellationToken cancellationToken)
        => dbContext.InvestmentProducts.AddAsync(product, cancellationToken).AsTask();
}

internal sealed class InvestmentOrderRepository(AppDbContext dbContext) : IInvestmentOrderRepository
{
    public Task AddAsync(InvestmentOrder order, CancellationToken cancellationToken)
        => dbContext.InvestmentOrders.AddAsync(order, cancellationToken).AsTask();

    public Task<InvestmentOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.InvestmentOrders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<InvestmentOrder>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        => await dbContext.InvestmentOrders.AsNoTracking().Where(x => x.CustomerId == customerId).OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
}

internal sealed class PortfolioPositionRepository(AppDbContext dbContext) : IPortfolioPositionRepository
{
    public async Task<IReadOnlyCollection<PortfolioPosition>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        => await dbContext.PortfolioPositions.AsNoTracking().Where(x => x.CustomerId == customerId).OrderByDescending(x => x.UpdatedAt).ToListAsync(cancellationToken);

    public Task<PortfolioPosition?> GetByCustomerAndProductAsync(Guid customerId, Guid productId, CancellationToken cancellationToken)
        => dbContext.PortfolioPositions.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.ProductId == productId, cancellationToken);

    public Task AddAsync(PortfolioPosition position, CancellationToken cancellationToken)
        => dbContext.PortfolioPositions.AddAsync(position, cancellationToken).AsTask();
}

internal sealed class ProcessedMessageRepository(AppDbContext dbContext) : IProcessedMessageRepository
{
    public Task<bool> ExistsAsync(Guid messageId, string consumerName, CancellationToken cancellationToken)
        => dbContext.ProcessedMessages.AnyAsync(x => x.MessageId == messageId && x.ConsumerName == consumerName, cancellationToken);

    public Task AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken)
        => dbContext.ProcessedMessages.AddAsync(processedMessage, cancellationToken).AsTask();
}

internal sealed class OutboxRepository(AppDbContext dbContext) : IOutboxRepository
{
    public Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        => dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken).AsTask();

    public async Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int take, CancellationToken cancellationToken)
        => await dbContext.OutboxMessages.Where(x => x.Status == "Pending").OrderBy(x => x.OccurredOn).Take(take).ToListAsync(cancellationToken);
}

internal sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
