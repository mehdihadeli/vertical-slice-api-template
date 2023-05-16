using FluentValidation;
using Vertical.Slice.Template.Shared.Core.Domain.Events;
using Vertical.Slice.Template.Shared.Validation.Extensions;

namespace Vertical.Slice.Template.Products.Features.CreatingProduct.v1;

// https://event-driven.io/en/explicit_validation_in_csharp_just_got_simpler/
// https://event-driven.io/en/how_to_validate_business_logic/
// https://event-driven.io/en/notes_about_csharp_records_and_nullable_reference_types/
// https://buildplease.com/pages/vos-in-events/
// https://codeopinion.com/leaking-value-objects-from-your-domain/
// https://www.youtube.com/watch?v=CdanF8PWJng
// we don't pass value-objects and domains to our commands and events, just primitive types
public record ProductCreated(Guid Id, string Name, Guid CategoryId, decimal Price, string? Description = null)
    : DomainEvent
{
    public static ProductCreated Of(Guid id, string? name, Guid categoryId, decimal price, string? description = null)
    {
        return new ProductCreatedValidator().HandleValidation(
            new ProductCreated(id, name!, categoryId, price, description)
        );

        // // Also if validation rules are simple you can just validate inputs explicitly
        // id.NotBeEmpty();
        // name.NotBeNullOrWhiteSpace();
        // categoryId.NotBeEmpty();
        // price.NotBeNegativeOrZero();
        // return new ProductCreated(id, name, categoryId, price, description);
    }
}

internal class ProductCreatedValidator : AbstractValidator<ProductCreated>
{
    public ProductCreatedValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MinimumLength(3);
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.CategoryId).NotEmpty();
        RuleFor(r => r.Price).NotEmpty().GreaterThan(0);
    }
}
