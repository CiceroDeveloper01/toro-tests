using MediatR;
using Toro.Testes.Contracts.Common;
using Toro.Testes.Domain.Entities;

namespace Toro.Testes.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}

public interface IInvestmentProductRepository
{
    Task<IReadOnlyCollection<InvestmentProduct>> GetAllAsync(CancellationToken cancellationToken);
    Task<InvestmentProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(InvestmentProduct product, CancellationToken cancellationToken);
}

public interface IInvestmentOrderRepository
{
    Task AddAsync(InvestmentOrder order, CancellationToken cancellationToken);
    Task<InvestmentOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<InvestmentOrder>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
}

public interface IPortfolioPositionRepository
{
    Task<IReadOnlyCollection<PortfolioPosition>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<PortfolioPosition?> GetByCustomerAndProductAsync(Guid customerId, Guid productId, CancellationToken cancellationToken);
    Task AddAsync(PortfolioPosition position, CancellationToken cancellationToken);
}

public interface IProcessedMessageRepository
{
    Task<bool> ExistsAsync(Guid messageId, string consumerName, CancellationToken cancellationToken);
    Task AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken);
}

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int take, CancellationToken cancellationToken);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IJwtTokenService
{
    string GenerateToken(Customer customer);
}

public interface IMessageBusPublisher
{
    Task PublishAsync<T>(MessageEnvelope<T> envelope, string routingKey, CancellationToken cancellationToken);
}

public interface ICurrentUserService
{
    Guid? CustomerId { get; }
    string? Email { get; }
    string? Role { get; }
}

public interface IOutboxSerializer
{
    OutboxMessage Create<T>(string eventName, MessageEnvelope<T> envelope);
}

public interface IInvestmentOrderProcessingService
{
    Task ProcessAsync(MessageEnvelope<Toro.Testes.Contracts.Events.InvestmentOrderCreatedIntegrationEvent> envelope, CancellationToken cancellationToken);
}
