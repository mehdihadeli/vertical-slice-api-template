using Shared.Abstractions.Core.Paging;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Clients.Dtos;

namespace Vertical.Slice.Template.Shared.Clients;

// Ref: https://learn.microsoft.com/en-us/azure/architecture/patterns/anti-corruption-layer
// Ref: https://deviq.com/domain-driven-design/anti-corruption-layer

/// <summary>
/// ICatalogsClient acts as an anti-corruption-layer for our system.
/// An Anti-Corruption Layer (ACL) is a set of patterns placed between the domain model and other bounded contexts or third party dependencies. The intent of this layer is to prevent the intrusion of foreign concepts and models into the domain model.
/// </summary>
public interface ICatalogsClient
{
    /// <param name="createProductClientRequestDto">CreateProductInput.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Creating a New Product.
    /// </summary>
    /// <returns>Product created successfully.</returns>
    Task<Guid> CreateProductAsync(
        CreateProductClientRequestDto createProductClientRequestDto,
        CancellationToken cancellationToken
    );

    /// <param name="getProductsByPageClientRequestDto">Input data for paging and filtering.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Getting products by page info.
    /// </summary>
    /// <returns>Products fetched successfully.</returns>
    Task<IPageList<Product>> GetProductByPageAsync(
        GetProductsByPageClientRequestDto getProductsByPageClientRequestDto,
        CancellationToken cancellationToken
    );

    /// <param name="id">Id.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Getting a product by id.
    /// </summary>
    /// <returns>Product fetched successfully.</returns>
    Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
}

public interface ICatalogsRestClient : ICatalogsClient;

public interface ICatalogsConnectedServiceClient : ICatalogsClient;

public interface ICatalogsKiotaClient : ICatalogsClient;
