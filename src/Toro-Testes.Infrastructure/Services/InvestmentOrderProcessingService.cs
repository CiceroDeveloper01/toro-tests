using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Abstractions;
using Toro.Testes.BuildingBlocks.Constants;
using Toro.Testes.Contracts.Common;
using Toro.Testes.Contracts.Events;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Domain.Enums;

namespace Toro.Testes.Infrastructure.Services;

internal sealed class InvestmentOrderProcessingService(
    IInvestmentOrderRepository orderRepository,
    IInvestmentProductRepository productRepository,
    IPortfolioPositionRepository portfolioRepository,
    IProcessedMessageRepository processedMessageRepository,
    IOutboxRepository outboxRepository,
    IOutboxSerializer outboxSerializer,
    IUnitOfWork unitOfWork,
    IClock clock) : IInvestmentOrderProcessingService
{
    public async Task ProcessAsync(MessageEnvelope<InvestmentOrderCreatedIntegrationEvent> envelope, CancellationToken cancellationToken)
    {
        if (await processedMessageRepository.ExistsAsync(envelope.MessageId, ApplicationConstants.Messaging.WorkerConsumerName, cancellationToken))
        {
            return;
        }

        var order = await orderRepository.GetByIdAsync(envelope.Payload.OrderId, cancellationToken)
            ?? throw new InvalidOperationException("Order not found during processing.");

        var product = await productRepository.GetByIdAsync(envelope.Payload.ProductId, cancellationToken)
            ?? throw new InvalidOperationException("Product not found during processing.");

        order.EnsureCanBeProcessed();
        order.StartProcessing();

        if (!product.IsActive || order.Amount > 1_000_000)
        {
            order.Reject("Order rejected by risk validation.");
            var failedEnvelope = new MessageEnvelope<InvestmentOrderFailedIntegrationEvent>
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = envelope.CorrelationId,
                EventName = "investment-order-failed",
                OccurredAt = clock.UtcNow,
                Payload = new InvestmentOrderFailedIntegrationEvent(order.Id, order.CustomerId, order.ProductId, order.Amount, order.Status.ToString(), order.RejectionReason!, clock.UtcNow)
            };
            await outboxRepository.AddAsync(outboxSerializer.Create("investment-order-failed", failedEnvelope), cancellationToken);
        }
        else
        {
            order.Complete();
            var position = await portfolioRepository.GetByCustomerAndProductAsync(order.CustomerId, order.ProductId, cancellationToken);
            if (position is null)
            {
                position = PortfolioPosition.Create(order.CustomerId, order.ProductId, order.Amount, 1, product.AnnualInterestRate);
                await portfolioRepository.AddAsync(position, cancellationToken);
            }
            else
            {
                position.AddInvestment(order.Amount, product.AnnualInterestRate);
            }

            var processedEnvelope = new MessageEnvelope<InvestmentOrderProcessedIntegrationEvent>
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = envelope.CorrelationId,
                EventName = "investment-order-processed",
                OccurredAt = clock.UtcNow,
                Payload = new InvestmentOrderProcessedIntegrationEvent(order.Id, order.CustomerId, order.ProductId, order.Amount, OrderStatus.Completed.ToString(), clock.UtcNow)
            };
            await outboxRepository.AddAsync(outboxSerializer.Create("investment-order-processed", processedEnvelope), cancellationToken);
        }

        await processedMessageRepository.AddAsync(ProcessedMessage.Create(envelope.MessageId, ApplicationConstants.Messaging.WorkerConsumerName, clock.UtcNow), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
