using FluentValidation;
using Shared.Core.Domain;
using Shared.Validation;

namespace Vertical.Slice.Template.Products.Features.CreatingProduct.v1;

public record ProductCreated(Guid Id, string Name, Guid CategoryId, decimal Price, string? Description = null)
    : DomainEvent
{
    public static ProductCreated Of(Guid id, string name, Guid categoryId, decimal price, string? description = null)
    {
        return new ProductCreatedValidator().HandleValidation(
            new ProductCreated(id, name, categoryId, price, description)
        );
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
