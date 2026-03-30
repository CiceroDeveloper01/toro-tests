using System.Text.Json;
using FluentValidation;
using MediatR;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Results;
using Toro.Testes.Contracts.Common;
using Toro.Testes.Contracts.Events;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Domain.ValueObjects;

namespace Toro.Testes.Application.Features.InvestmentOrders.Commands.CreateInvestmentOrder;

public sealed record CreateInvestmentOrderCommand(Guid CustomerId, Guid ProductId, decimal Amount, string CorrelationId)
    : IRequest<Result<CreateInvestmentOrderResponse>>;

public sealed class CreateInvestmentOrderCommandValidator : AbstractValidator<CreateInvestmentOrderCommand>
{
    public CreateInvestmentOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.CorrelationId).NotEmpty();
    }
}

public sealed class CreateInvestmentOrderCommandHandler(
    ICustomerRepository customerRepository,
    IInvestmentProductRepository productRepository,
    IInvestmentOrderRepository orderRepository,
    IOutboxRepository outboxRepository,
    IOutboxSerializer outboxSerializer,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateInvestmentOrderCommand, Result<CreateInvestmentOrderResponse>>
{
    public async Task<Result<CreateInvestmentOrderResponse>> Handle(CreateInvestmentOrderCommand request, CancellationToken cancellationToken)
    {
        _ = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new Toro.Testes.BuildingBlocks.Exceptions.NotFoundException("Customer not found.");

        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new Toro.Testes.BuildingBlocks.Exceptions.NotFoundException("Investment product not found.");

        product.EnsureMinimumAmount(request.Amount);

        var order = InvestmentOrder.Create(request.CustomerId, request.ProductId, new Money(request.Amount), request.CorrelationId);
        await orderRepository.AddAsync(order, cancellationToken);

        var integrationEvent = new InvestmentOrderCreatedIntegrationEvent(order.Id, order.CustomerId, order.ProductId, order.Amount, order.Status.ToString());
        var envelope = new MessageEnvelope<InvestmentOrderCreatedIntegrationEvent>
        {
            MessageId = Guid.NewGuid(),
            CorrelationId = request.CorrelationId,
            EventName = "investment-order-created",
            OccurredAt = DateTimeOffset.UtcNow,
            Payload = integrationEvent
        };

        await outboxRepository.AddAsync(outboxSerializer.Create("investment-order-created", envelope), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreateInvestmentOrderResponse>.Success(
            new CreateInvestmentOrderResponse(order.Id, order.Status, "Investment order accepted for processing.", request.CorrelationId));
    }
}
