using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Abstractions.Core.CQRS;
using Vertical.Slice.Template.Shared.Abstractions.Ef;
using Vertical.Slice.Template.Shared.Core.Extensions;
using Vertical.Slice.Template.Shared.Core.Id;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.Shared.EF.Extensions;
using Vertical.Slice.Template.Shared.Validation.Extensions;

namespace Vertical.Slice.Template.Products.Features.CreatingProduct.v1;

// https://event-driven.io/en/explicit_validation_in_csharp_just_got_simpler/
// https://event-driven.io/en/how_to_validate_business_logic/
// https://event-driven.io/en/notes_about_csharp_records_and_nullable_reference_types/
// https://buildplease.com/pages/vos-in-events/
// https://codeopinion.com/leaking-value-objects-from-your-domain/
// https://www.youtube.com/watch?v=CdanF8PWJng
// we don't pass value-objects and domains to our commands and events, just primitive types
public record CreateProduct(string Name, Guid CategoryId, decimal Price, string? Description = null)
    : ICommand<CreateProductResult>
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

internal class CreateProductHandler : ICommandHandler<CreateProduct, CreateProductResult>
{
    private readonly DbExecuters.CreateAndSaveProductExecutor _createAndSaveProductExecutor;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        DbExecuters.CreateAndSaveProductExecutor createAndSaveProductExecutor,
        IMapper mapper,
        IMediator mediator,
        ILogger<CreateProductHandler> logger
    )
    {
        _createAndSaveProductExecutor = createAndSaveProductExecutor;
        _mapper = mapper;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<CreateProductResult> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        request.NotBeNull();

        var product = _mapper.Map<Product>(request);

        await _createAndSaveProductExecutor(product, cancellationToken);

        var (name, categoryId, price, description) = request;
        await _mediator.Publish(ProductCreated.Of(request.Id, name, categoryId, price, description), cancellationToken);

        _logger.LogInformation("Product a with ID: '{ProductId} created.'", request.Id);

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
