using AutoMapper;
using FluentValidation;
using MediatR;
using Shared.Core;
using Shared.Core.Extensions;
using Shared.Core.Id;
using Shared.EF.Extensions;
using Shared.Validation.Extensions;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Data;

namespace Vertical.Slice.Template.Products.Features.CreatingProduct.v1;

internal record CreateProduct(string Name, Guid CategoryId, decimal Price, string? Description = null)
    : IRequest<CreateProductResult>
{
    public Guid Id { get; } = IdGenerator.NewId();

    /// <summary>
    /// CreateProduct command with validation.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="categoryId"></param>
    /// <param name="price"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static CreateProduct Of(string? name, Guid categoryId, decimal price, string? description = null)
    {
        return new CreateProductValidator().HandleValidation(new CreateProduct(name!, categoryId, price, description));
    }
}

internal class CreateProductValidator : AbstractValidator<CreateProduct>
{
    public CreateProductValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MinimumLength(3);
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.CategoryId).NotEmpty();
        RuleFor(r => r.Price).NotEmpty().GreaterThan(0);
    }
}

internal class CreateProductHandler : IRequestHandler<CreateProduct, CreateProductResult>
{
    private readonly DbExecuters.CreateAndSaveProductExecutor _createAndSaveProductExecutor;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateProductHandler(
        DbExecuters.CreateAndSaveProductExecutor createAndSaveProductExecutor,
        IMapper mapper,
        IMediator mediator
    )
    {
        _createAndSaveProductExecutor = createAndSaveProductExecutor;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<CreateProductResult> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        request.NotBeNull();

        var product = _mapper.Map<Product>(request);

        await _createAndSaveProductExecutor(product, cancellationToken);

        var (name, categoryId, price, description) = request;
        await _mediator.Publish(ProductCreated.Of(request.Id, name, categoryId, price, description), cancellationToken);

        return new CreateProductResult(product.Id);
    }
}

internal record CreateProductResult(Guid Id);

internal class DbExecuters : IDbExecutors
{
    public delegate ValueTask CreateAndSaveProductExecutor(Product product, CancellationToken cancellationToken);

    public void Register(IServiceCollection services)
    {
        // Db related operations for injection as dependencies
        services.AddTransient<CreateAndSaveProductExecutor>(sp =>
        {
            var context = sp.GetRequiredService<CatalogsDbContext>();
            return (entity, cancellationToken) => context.InsertAndSaveAsync(entity, cancellationToken);
        });
    }
}
