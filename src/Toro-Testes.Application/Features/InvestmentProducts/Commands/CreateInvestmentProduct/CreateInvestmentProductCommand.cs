using FluentValidation;
using MediatR;
using Toro.Testes.Application.DTOs.Responses;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Results;
using Toro.Testes.Domain.Entities;
using Toro.Testes.Domain.Enums;
using Toro.Testes.Domain.ValueObjects;

namespace Toro.Testes.Application.Features.InvestmentProducts.Commands.CreateInvestmentProduct;

public sealed record CreateInvestmentProductCommand(
    string Name,
    InvestmentType InvestmentType,
    decimal MinimumAmount,
    decimal AnnualInterestRate,
    ProductLiquidityType LiquidityType,
    RiskLevel RiskLevel,
    DateOnly MaturityDate,
    int? GracePeriodDays,
    bool IsActive) : IRequest<Result<CreateInvestmentProductResponse>>;

public sealed class CreateInvestmentProductCommandValidator : AbstractValidator<CreateInvestmentProductCommand>
{
    public CreateInvestmentProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.MinimumAmount).GreaterThan(0);
        RuleFor(x => x.AnnualInterestRate).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.MaturityDate).Must(date => date > DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.GracePeriodDays)
            .GreaterThan(0)
            .When(x => x.InvestmentType is InvestmentType.LCI or InvestmentType.LCA);
    }
}

public sealed class CreateInvestmentProductCommandHandler(IInvestmentProductRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateInvestmentProductCommand, Result<CreateInvestmentProductResponse>>
{
    public async Task<Result<CreateInvestmentProductResponse>> Handle(CreateInvestmentProductCommand request, CancellationToken cancellationToken)
    {
        var product = InvestmentProduct.Create(
            request.Name,
            request.InvestmentType,
            new Money(request.MinimumAmount),
            new Percentage(request.AnnualInterestRate),
            request.LiquidityType,
            request.RiskLevel,
            new MaturityDate(request.MaturityDate),
            request.GracePeriodDays,
            request.IsActive);

        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreateInvestmentProductResponse>.Success(new CreateInvestmentProductResponse(product.Id, "Investment product created successfully."));
    }
}
