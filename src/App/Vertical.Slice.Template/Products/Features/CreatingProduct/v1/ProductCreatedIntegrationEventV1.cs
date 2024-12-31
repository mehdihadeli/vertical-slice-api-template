using Shared.Core.Extensions;
using Shared.Core.Messaging;

namespace Vertical.Slice.Template.Products.Features.CreatingProduct.v1;

public record ProductCreatedIntegrationEventV1(
    Guid Id,
    string Name,
    Guid CategoryId,
    decimal Price,
    string? Description = null
) : IntegrationEvent
{
    /// <summary>
    /// ProductCreatedV1 with in-line validation.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="categoryId"></param>
    /// <param name="price"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static ProductCreatedIntegrationEventV1 Of(
        Guid id,
        string? name,
        Guid categoryId,
        decimal price,
        string? description = null
    )
    {
        id.NotBeEmpty();
        name.NotBeNullOrWhiteSpace();
        categoryId.NotBeEmpty();
        price.NotBeNegativeOrZero();

        return new ProductCreatedIntegrationEventV1(id, name, categoryId, price, description);
    }
}
