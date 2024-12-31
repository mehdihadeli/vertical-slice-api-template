using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Polly.Wrap;
using Shared.Abstractions.Core.Paging;
using Shared.Core.Exceptions;
using Shared.Core.Paging;
using Shared.Web.Extensions;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Clients.Dtos;

namespace Vertical.Slice.Template.Shared.Clients.Rest;

public class CatalogsRestClient(
    HttpClient httpClient,
    AsyncPolicyWrap combinedPolicy,
    IOptions<CatalogsRestClientOptions> options
) : ICatalogsRestClient
{
    private readonly CatalogsRestClientOptions _options = options.Value;

    public async Task<Guid> CreateProductAsync(
        CreateProductClientRequestDto createProductClientRequestDto,
        CancellationToken cancellationToken
    )
    {
        var response = await combinedPolicy.ExecuteAsync(async () =>
        {
            var response = await httpClient.PostAsJsonAsync(
                _options.CreateProductEndpoint,
                createProductClientRequestDto,
                cancellationToken
            );

            // https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
            // throw HttpResponseException instead of HttpRequestException (because we want detail response exception) with corresponding status code
            await response.EnsureSuccessStatusCodeWithDetailAsync();

            var result = await response.Content.ReadFromJsonAsync<CreateProductClientResponseDto>(
                cancellationToken: cancellationToken
            );

            return result;
        });

        if (response?.Id == null)
        {
            throw new InvalidOperationException("The response did not contain a valid product ID.");
        }

        return response.Id;
    }

    public async Task<IPageList<Product>> GetProductByPageAsync(
        GetProductsByPageClientRequestDto getProductsByPageClientRequestDto,
        CancellationToken cancellationToken
    )
    {
        // https://stackoverflow.com/a/67877742/581476
        var qb = new QueryBuilder
        {
            { "limit", getProductsByPageClientRequestDto.PageSize.ToString(CultureInfo.InvariantCulture) },
            { "skip", getProductsByPageClientRequestDto.PageNumber.ToString(CultureInfo.InvariantCulture) },
        };

        // https://github.com/App-vNext/Polly#handing-return-values-and-policytresult
        var getProductsByPageResponse = await combinedPolicy.ExecuteAsync(async () =>
        {
            // https://ollama.com/blog/openai-compatibility
            // https://www.youtube.com/watch?v=38jlvmBdBrU
            // https://platform.openai.com/docs/api-reference/chat/create
            // https://github.com/ollama/ollama/blob/main/docs/api.md#generate-a-chat-completion
            var response = await httpClient.GetFromJsonAsync<GetProductByPageClientResponseDto>(
                $"{_options.GetProductByPageEndpoint}?{qb.ToQueryString().Value}",
                cancellationToken
            );

            return response;
        });

        if (
            getProductsByPageResponse is null
            || getProductsByPageResponse.Products is null
            || getProductsByPageResponse.Products.Items is null
        )
            throw new Exception("products page list cannot be null");

        var pageList = getProductsByPageResponse.Products.MapTo(x => x.ToProduct());

        return pageList;
    }

    public async Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await combinedPolicy.ExecuteAsync(async () =>
        {
            var response = await httpClient.GetFromJsonAsync<GetProductByIdClientResponseDto>(
                $"{_options.GetProductByIdEndpoint}/{id}",
                cancellationToken
            );

            return response;
        });

        var product = response?.Product?.ToProduct();

        if (product is null)
            throw new NotFoundException($"product with id '{id}' not found");

        return product;
    }
}
