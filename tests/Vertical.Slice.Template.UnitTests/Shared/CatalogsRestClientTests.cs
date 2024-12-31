using AutoBogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shared.Core.Exceptions;
using Shared.Core.Paging;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Clients.Dtos;
using Vertical.Slice.Template.Shared.Clients.Rest;
using Vertical.Slice.Template.UnitTests.Common;

namespace Vertical.Slice.Template.UnitTests.Shared;

public class CatalogsRestClientTests : CatalogsUnitTestBase
{
    [Fact]
    public async Task CreateProductAsync_ShouldCallHttpClientWithValidParametersOnce()
    {
        // Arrange
        var createProductDto = new AutoFaker<CreateProductClientRequestDto>().Generate();
        var responseDto = new CreateProductClientResponseDto { Id = Guid.NewGuid() };

        var apiClientOptions = Options.Create(
            new CatalogsRestClientOptions
            {
                BaseAddress = "http://localhost",
                CreateProductEndpoint = "api/v1/products",
            }
        );

        var url =
            $"{
            apiClientOptions.Value.BaseAddress
        }/{
            apiClientOptions.Value.CreateProductEndpoint
        }*";

        // https://github.com/richardszalay/mockhttp
        // https://code-maze.com/csharp-mock-httpclient-with-unit-tests/
        var mockHttp = new MockHttpMessageHandler();

        // Setup a response for the user api (including a wildcard in the URL)
        var request = mockHttp.When(url).Respond("application/json", JsonConvert.SerializeObject(responseDto));

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(apiClientOptions.Value.BaseAddress);
        var catalogsRestClient = new CatalogsRestClient(client, CombinedPolicy, apiClientOptions);

        // Act
        var result = await catalogsRestClient.CreateProductAsync(createProductDto, CancellationToken.None);

        // Assert
        result.Should().Be(responseDto.Id);
        mockHttp.GetMatchCount(request).Should().Be(1);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var responseDto = new GetProductByIdClientResponseDto(new AutoFaker<ProductClientDto>().Generate());

        var apiClientOptions = Options.Create(
            new CatalogsRestClientOptions
            {
                BaseAddress = "http://localhost",
                GetProductByIdEndpoint = "api/v1/products",
            }
        );

        var url =
            $"{
                apiClientOptions.Value.BaseAddress
            }/{
                apiClientOptions.Value.GetProductByIdEndpoint
            }/{productId}";

        var mockHttp = new MockHttpMessageHandler();

        var request = mockHttp.When(url).Respond("application/json", JsonConvert.SerializeObject(responseDto));

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(apiClientOptions.Value.BaseAddress);
        var catalogsRestClient = new CatalogsRestClient(client, CombinedPolicy, apiClientOptions);

        // Act
        var result = await catalogsRestClient.GetProductByIdAsync(productId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(responseDto.Product.Id);
        mockHttp.GetMatchCount(request).Should().Be(1);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldThrowNotFoundException_WhenProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var apiClientOptions = Options.Create(
            new CatalogsRestClientOptions
            {
                BaseAddress = "http://localhost",
                GetProductByIdEndpoint = "api/v1/products",
            }
        );

        var url = $"{apiClientOptions.Value.BaseAddress}/{apiClientOptions.Value.GetProductByIdEndpoint}/{productId}";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(url).Throw(new NotFoundException($"product with id {productId} not found"));

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(apiClientOptions.Value.BaseAddress);
        var catalogsRestClient = new CatalogsRestClient(client, CombinedPolicy, apiClientOptions);

        // Act
        Func<Task> action = async () => await catalogsRestClient.GetProductByIdAsync(productId, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"product with id {productId} not found");
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldThrowHttpRequestException_WhenHttpClientFails()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var apiClientOptions = Options.Create(
            new CatalogsRestClientOptions
            {
                BaseAddress = "http://localhost",
                GetProductByIdEndpoint = "api/v1/products",
            }
        );

        var url = $"{apiClientOptions.Value.BaseAddress}/{apiClientOptions.Value.GetProductByIdEndpoint}/{productId}";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(url).Throw(new HttpRequestException("Network error"));

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(apiClientOptions.Value.BaseAddress);
        var catalogsRestClient = new CatalogsRestClient(client, CombinedPolicy, apiClientOptions);

        // Act
        Func<Task> action = async () => await catalogsRestClient.GetProductByIdAsync(productId, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>().WithMessage("Network error");
    }

    [Fact]
    public async Task get_all_products_should_call_http_client_with_valid_parameters_once()
    {
        // Arrange
        var pageSize = 10;
        var page = 1;
        var total = 20;

        var apiClientOptions = Options.Create(
            new CatalogsRestClientOptions
            {
                BaseAddress = "http://localhost",
                GetProductByPageEndpoint = "api/v1/products",
            }
        );

        var url =
            $"{
            apiClientOptions.Value.BaseAddress
        }/{
            apiClientOptions.Value.GetProductByPageEndpoint
        }*";

        var productClientDtos = new AutoFaker<ProductClientDto>().Generate(total);

        var getProductByPageClientResponseDto = new GetProductByPageClientResponseDto(
            new PageList<ProductClientDto>(productClientDtos, page, pageSize, total)
        );

        // https://github.com/richardszalay/mockhttp
        // https://code-maze.com/csharp-mock-httpclient-with-unit-tests/
        var mockHttp = new MockHttpMessageHandler();

        // Setup a response for the user api (including a wildcard in the URL)
        var request = mockHttp
            .When(url)
            .Respond("application/json", JsonConvert.SerializeObject(getProductByPageClientResponseDto));

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(apiClientOptions.Value.BaseAddress);

        var getProductsByPageClientRequestDto = new GetProductsByPageClientRequestDto
        {
            PageNumber = page,
            PageSize = pageSize,
        };

        var catalogsRestClient = new CatalogsRestClient(client, CombinedPolicy, apiClientOptions);

        // Act
        await catalogsRestClient.GetProductByPageAsync(getProductsByPageClientRequestDto, CancellationToken.None);

        // Assert
        mockHttp.GetMatchCount(request).Should().Be(1);
    }

    [Fact]
    public async Task get_all_products_should_return_products_list()
    {
        // Arrange
        var pageSize = 10;
        var page = 1;
        var total = 20;

        var apiClientOptions = Options.Create(
            new CatalogsRestClientOptions
            {
                BaseAddress = "http://localhost",
                GetProductByPageEndpoint = "api/v1/products",
            }
        );

        var url =
            $"{
            apiClientOptions.Value.BaseAddress
        }/{
            apiClientOptions.Value.GetProductByPageEndpoint
        }*";

        var productClientDtos = new AutoFaker<ProductClientDto>().Generate(total);

        var getProductByPageClientResponseDto = new GetProductByPageClientResponseDto(
            new PageList<ProductClientDto>(productClientDtos, page, pageSize, total)
        );

        // https://github.com/richardszalay/mockhttp
        // https://code-maze.com/csharp-mock-httpclient-with-unit-tests/
        var mockHttp = new MockHttpMessageHandler();

        // Setup a response for the user api (including a wildcard in the URL)
        var request = mockHttp
            .When(url)
            .Respond("application/json", JsonConvert.SerializeObject(getProductByPageClientResponseDto));

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(apiClientOptions.Value.BaseAddress);

        var getProductsByPageClientRequestDto = new GetProductsByPageClientRequestDto
        {
            PageNumber = page,
            PageSize = pageSize,
        };

        var products = productClientDtos.ToProducts();
        var expectedPageList = new PageList<Product>(products.ToList(), page, pageSize, total);

        var catalogsRestClient = new CatalogsRestClient(client, CombinedPolicy, apiClientOptions);

        // Act
        var result = await catalogsRestClient.GetProductByPageAsync(
            getProductsByPageClientRequestDto,
            CancellationToken.None
        );

        // Assert
        result.Should().BeEquivalentTo(expectedPageList, c => c.ExcludingMissingMembers());
    }

    [Fact]
    public async Task get_all_products_with_http_response_exception_should_throw_http_response_exception()
    {
        // Arrange
        var pageSize = 10;
        var page = 1;

        var apiClientOptions = Options.Create(
            new CatalogsRestClientOptions
            {
                BaseAddress = "http://localhost",
                GetProductByPageEndpoint = "api/v1/products",
            }
        );

        var url =
            $"{
            apiClientOptions.Value.BaseAddress
        }/{
            apiClientOptions.Value.GetProductByPageEndpoint
        }*";

        // https://github.com/richardszalay/mockhttp
        // https://code-maze.com/csharp-mock-httpclient-with-unit-tests/
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Fallback.Throw(
            new HttpResponseException(StatusCodes.Status500InternalServerError, "There is an error in the server")
        );

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(apiClientOptions.Value.BaseAddress);

        var getProductsByPageClientRequestDto = new GetProductsByPageClientRequestDto
        {
            PageNumber = page,
            PageSize = pageSize,
        };

        var catalogsRestClient = new CatalogsRestClient(client, CombinedPolicy, apiClientOptions);

        // Act
        Func<Task> act = () =>
            catalogsRestClient.GetProductByPageAsync(getProductsByPageClientRequestDto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<HttpResponseException>();
    }

    [Fact]
    public async Task get_all_products_with_exception_should_throw_exception()
    {
        // Arrange
        var pageSize = 10;
        var page = 1;

        var apiClientOptions = Options.Create(
            new CatalogsRestClientOptions
            {
                BaseAddress = "http://localhost",
                GetProductByPageEndpoint = "api/v1/products",
            }
        );

        var url =
            $"{
            apiClientOptions.Value.BaseAddress
        }/{
            apiClientOptions.Value.GetProductByPageEndpoint
        }*";

        // https://github.com/richardszalay/mockhttp
        // https://code-maze.com/csharp-mock-httpclient-with-unit-tests/
        var mockHttp = new MockHttpMessageHandler();

        // Setup a response for the user api (including a wildcard in the URL)
        var request = mockHttp.When(url).Respond("application/json", JsonConvert.SerializeObject(null)); // Respond with JSON

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(apiClientOptions.Value.BaseAddress);

        var getProductsByPageClientRequestDto = new GetProductsByPageClientRequestDto
        {
            PageNumber = page,
            PageSize = pageSize,
        };

        var catalogsRestClient = new CatalogsRestClient(client, CombinedPolicy, apiClientOptions);

        // Act
        Func<Task> act = () =>
            catalogsRestClient.GetProductByPageAsync(getProductsByPageClientRequestDto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}
