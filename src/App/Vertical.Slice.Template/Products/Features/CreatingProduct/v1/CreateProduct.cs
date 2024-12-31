using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Core.CQRS;
using Shared.Abstractions.Core.Messaging;
using Shared.Abstractions.Persistence.Ef;
using Shared.Core.Extensions;
using Shared.Core.Id;
using Shared.EF.Extensions;
using Shared.Validation.Extensions;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Data;

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

public class CreateProductValidator : AbstractValidator<CreateProduct>
{
    public CreateProductValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MinimumLength(3);
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.CategoryId).NotEmpty();
        RuleFor(r => r.Price).NotEmpty().GreaterThan(0);
    }
}

internal class CreateProductHandler(
    DbExecuters.CreateAndSaveProductExecutor createAndSaveProductExecutor,
    IMediator mediator,
    IExternalEventBus externalEventBus,
    ILogger<CreateProductHandler> logger
) : ICommandHandler<CreateProduct, CreateProductResult>
{
    public async ValueTask<CreateProductResult> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        request.NotBeNull();

        var product = request.ToProduct();

        await createAndSaveProductExecutor(product, cancellationToken);

        var (name, categoryId, price, description) = request;
        await mediator.Publish(
            ProductCreatedDomainEvent.Of(request.Id, name, categoryId, price, description),
            cancellationToken
        );

        // publish integration event to external broker
        await externalEventBus.PublishAsync(
            message: ProductCreatedIntegrationEventV1.Of(request.Id, name, categoryId, price, description),
            metadataHeaders: null,
            cancellationToken: cancellationToken
        );

        logger.LogInformation("Product a with ID: '{ProductId} created.'", request.Id);

        return new CreateProductResult(product.Id);
    }
}

public record CreateProductResult(Guid Id);

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
