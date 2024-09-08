using Catalogs.ApiClient;
using Shared.Abstractions.Core.Paging;
using Shared.Core.Exceptions;
using Shared.Core.Extensions;
using Shared.Core.Paging;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Clients.Catalogs.Dtos;

namespace Vertical.Slice.Template.Shared.Clients.Catalogs;

public class CatalogsClient(ICatalogsApiClient catalogsApiClient) : ICatalogsClient
{
    public async Task<Guid> CreateProductAsync(
        CreateProductClientDto createProductClientDto,
        CancellationToken cancellationToken
    )
    {
        createProductClientDto.NotBeNull();

        try
        {
            // https://github.com/App-vNext/Polly#post-execution-capturing-the-result-or-any-final-exception
            // https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
            // https: //github.com/App-vNext/Polly#step-1--specify-the--exceptionsfaults-you-want-the-policy-to-handle
            var response = await catalogsApiClient.CreateProductAsync(
                new CreateProductRequest(
                    createProductClientDto.CategoryId,
                    createProductClientDto.Description,
                    createProductClientDto.Name,
                    (double)createProductClientDto.Price
                ),
                cancellationToken
            );

            var id = response.Id;

            return id;
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
        GetGetProductsByPageClientDto getProductsByPageClientDto,
        CancellationToken cancellationToken
    )
    {
        getProductsByPageClientDto.NotBeNull();

        try
        {
            // https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
            // https: //github.com/App-vNext/Polly#step-1--specify-the--exceptionsfaults-you-want-the-policy-to-handle
            var response = await catalogsApiClient.GetProductsByPageAsync(
                getProductsByPageClientDto.PageSize,
                getProductsByPageClientDto.PageNumber,
                getProductsByPageClientDto.Filters,
                getProductsByPageClientDto.SortOrder,
                cancellationToken
            );

            var items = response.Products!.Items.ToProducts();

            return PageList<Product>.Create(
                items.AsReadOnly(),
                response.Products.PageNumber,
                response.Products.PageSize,
                response.Products.TotalCount
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
            // https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
            // https: //github.com/App-vNext/Polly#step-1--specify-the--exceptionsfaults-you-want-the-policy-to-handle
            var response = await catalogsApiClient.GetProductByIdAsync(id, cancellationToken);

            var product = response.Product.ToProduct();

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
