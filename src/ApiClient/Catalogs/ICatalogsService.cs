using ApiClient.Catalogs.Dtos;

namespace ApiClient.Catalogs;

/// <summary>
/// Anti corruption layer fo catalogs service.
/// </summary>
public interface ICatalogsService
{
    /// <param name="createProductInput">CreateProductInput.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Creating a New Product.
    /// </summary>
    /// <returns>Product created successfully.</returns>
    Task<CreateProductOutput> CreateProductAsync(
        CreateProductInput createProductInput,
        CancellationToken cancellationToken
    );

    /// <param name="getProductsByPageInput">Input data for paging and filtering.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Getting products by page info.
    /// </summary>
    /// <returns>Products fetched successfully.</returns>
    Task<GetGetProductsByPageOutput> GetProductByPageAsync(
        GetGetProductsByPageInput getProductsByPageInput,
        CancellationToken cancellationToken
    );

    /// <param name="id">Id.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Getting a product by id.
    /// </summary>
    /// <returns>Product fetched successfully.</returns>
    Task<GetProductByIdOutput> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
}
