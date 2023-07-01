using AutoMapper;
using Catalogs.ApiClient;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Abstractions.Core.Paging;
using Vertical.Slice.Template.Shared.Clients.Catalogs.Dtos;
using Vertical.Slice.Template.Shared.Core.Exceptions;
using Vertical.Slice.Template.Shared.Core.Extensions;
using Vertical.Slice.Template.Shared.Core.Paging;

namespace Vertical.Slice.Template.Shared.Clients.Catalogs;

public class CatalogsClient : ICatalogsClient
{
    private readonly ICatalogsApiClient _catalogsApiClient;
    private readonly IMapper _mapper;

    public CatalogsClient(ICatalogsApiClient catalogsApiClient, IMapper mapper)
    {
        _catalogsApiClient = catalogsApiClient;
        _mapper = mapper;
    }

    public const string ClientName = "CatalogsClient";

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
            var response = await _catalogsApiClient.CreateProductAsync(
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
            var response = await _catalogsApiClient.GetProductsByPageAsync(
                getProductsByPageClientDto.PageSize,
                getProductsByPageClientDto.PageNumber,
                getProductsByPageClientDto.Filters,
                getProductsByPageClientDto.SortOrder,
                cancellationToken
            );

            var items = _mapper.Map<List<Product>>(response.Products!.Items);

            return PageList<Product>.Create(
                items,
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
            var response = await _catalogsApiClient.GetProductByIdAsync(id, cancellationToken);

            var product = _mapper.Map<Product>(response.Product);

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
