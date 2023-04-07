using AutoMapper;
using Catalogs.Shared.Data;
using FluentValidation;
using MediatR;
using Shared.Core;
using Shared.Core.Exceptions;
using Shared.EF.Extensions;

namespace Catalogs.Products.Features.CreatingProduct.v1;

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
    private readonly DbExecuters.CreateAndSaveProductExecutor _createAndSaveProductExecutor;
    private readonly IValidator<CreateProduct> _validator;
    private readonly IMapper _mapper;

    public CreateProductHandler(
        DbExecuters.CreateAndSaveProductExecutor createAndSaveProductExecutor,
        IValidator<CreateProduct> validator,
        IMapper mapper
    )
    {
        _createAndSaveProductExecutor = createAndSaveProductExecutor;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<CreateProductResult> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new BadRequestException(string.Join(',', result.Errors.Select(x => x.ErrorMessage)));
        }

        var product = _mapper.Map<Product>(request);

        await _createAndSaveProductExecutor(product, cancellationToken);

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
