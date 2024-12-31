using Shared.Abstractions.Core.Paging;
using Shared.Core.Exceptions;
using Shared.Core.Extensions;
using Shared.Core.Paging;
using Vertical.Slice.Template.ConnectedServiceClients.Catalogs;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Clients.Dtos;

namespace Vertical.Slice.Template.Shared.Clients.ConnectedService;

// Ref: https://learn.microsoft.com/en-us/azure/architecture/patterns/anti-corruption-layer
// Ref: https://deviq.com/domain-driven-design/anti-corruption-layer

/// <summary>
/// ICatalogsClient acts as an anti-corruption-layer for our system.
/// An Anti-Corruption Layer (ACL) is a set of patterns placed between the domain model and other bounded contexts or third party dependencies. The intent of this layer is to prevent the intrusion of foreign concepts and models into the domain model.
/// </summary>
public class CatalogsConnectedServiceClient(ICatalogsConectedServiceApiClient catalogsApiClient)
    : ICatalogsConnectedServiceClient
{
    public async Task<Guid> CreateProductAsync(
        CreateProductClientRequestDto createProductClientRequestDto,
        CancellationToken cancellationToken
    )
    {
        createProductClientRequestDto.NotBeNull();

        try
        {
            // resiliency using `Microsoft.Extensions.Http.Resilience`
            // https://learn.microsoft.com/en-us/dotnet/core/resilience/?tabs=dotnet-cli
            // https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience
            var createProductResponse = await catalogsApiClient.CreateProductAsync(
                new CreateProductRequest(
                    createProductClientRequestDto.CategoryId,
                    createProductClientRequestDto.Description,
                    createProductClientRequestDto.Name,
                    (double)createProductClientRequestDto.Price
                ),
                cancellationToken
            );

            var id = createProductResponse?.Id;

            if (id is null)
            {
                throw new InvalidOperationException("The response did not contain a valid product ID.");
            }

            return id.Value;
        }
        catch (ApiException apiException)
        {
            throw new HttpResponseException(
                apiException.StatusCode,
                apiException.Response!,
                apiException.Headers,
                apiException
            );
        }
    }

    public async Task<IPageList<Product>> GetProductByPageAsync(
        GetProductsByPageClientRequestDto getProductsByPageClientRequestDto,
        CancellationToken cancellationToken
    )
    {
        getProductsByPageClientRequestDto.NotBeNull();

        try
        {
            // resiliency using `Microsoft.Extensions.Http.Resilience`
            // https://learn.microsoft.com/en-us/dotnet/core/resilience/?tabs=dotnet-cli
            // https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience
            var getProductsByPageResponse = await catalogsApiClient.GetProductsByPageAsync(
                getProductsByPageClientRequestDto.PageSize,
                getProductsByPageClientRequestDto.PageNumber,
                getProductsByPageClientRequestDto.Filters,
                getProductsByPageClientRequestDto.SortOrder,
                cancellationToken
            );

            if (
                getProductsByPageResponse is null
                || getProductsByPageResponse.Products is null
                || getProductsByPageResponse.Products.Items is null
            )
                throw new Exception("products page list cannot be null");

            var items = getProductsByPageResponse.Products!.Items.ToProducts();

            return PageList<Product>.Create(
                items.AsReadOnly(),
                getProductsByPageResponse.Products.PageNumber,
                getProductsByPageResponse.Products.PageSize,
                getProductsByPageResponse.Products.TotalCount
            );
        }
        catch (ApiException apiException)
        {
            throw new HttpResponseException(
                apiException.StatusCode,
                apiException.Response!,
                apiException.Headers,
                apiException
            );
        }
    }

    public async Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        id.NotBeNull();

        try
        {
            // resiliency using `Microsoft.Extensions.Http.Resilience`
            // https://learn.microsoft.com/en-us/dotnet/core/resilience/?tabs=dotnet-cli
            // https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience
            var getProductByIdResponse = await catalogsApiClient.GetProductByIdAsync(id, cancellationToken);

            var product = getProductByIdResponse?.Product?.ToProduct();

            if (product is null)
                throw new NotFoundException($"product with id '{id}' not found");

            return product;
        }
        catch (ApiException apiException)
        {
            throw new HttpResponseException(
                apiException.StatusCode,
                apiException.Response!,
                apiException.Headers,
                apiException
            );
        }
    }
}
