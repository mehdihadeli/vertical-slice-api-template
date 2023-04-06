using Catalogs.Products.Data.Executors;
using FluentValidation;
using MediatR;
using Shared.Core.Exceptions;

namespace Catalogs.Products.Features.CreatingProduct;

internal record CreateProduct(string Name, Guid CategoryId, decimal Price, string? Description = null)
    : IRequest<CreateProductResult>
{
    public Guid Id { get; } = Guid.NewGuid();
}

internal class Validator : AbstractValidator<CreateProduct>
{
    public Validator()
    {
        RuleFor(r => r.Name).NotEmpty().MinimumLength(3);
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.CategoryId).NotEmpty();
        RuleFor(r => r.Price).NotEmpty().GreaterThan(0);
    }
}

internal class CreateProductHandler : IRequestHandler<CreateProduct, CreateProductResult>
{
    private readonly CreateAndSaveProductExecutor _createAndSaveProductExecutor;
    private readonly IValidator<CreateProduct> _validator;

    public CreateProductHandler(
        CreateAndSaveProductExecutor createAndSaveProductExecutor,
        IValidator<CreateProduct> validator
    )
    {
        _createAndSaveProductExecutor = createAndSaveProductExecutor;
        _validator = validator;
    }

    public async Task<CreateProductResult> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new BadRequestException(string.Join(',', result.Errors.Select(x => x.ErrorMessage)));
        }

        var product = new Product
        {
            Id = request.Id,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
        };

        await _createAndSaveProductExecutor(product, cancellationToken);

        return new CreateProductResult(product.Id);
    }
}

internal record CreateProductResult(Guid Id);
