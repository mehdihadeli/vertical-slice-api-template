using AutoBogus;
using Catalogs.ApiClient;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Vertical.Slice.Template.Api;

namespace Vertical.Slice.Template.ContractTests.Products;

public class GetProductByIdTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _appFactory;

    public GetProductByIdTests(WebApplicationFactory<Program> appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task BasicContractTest()
    {
        using var baseClient = _appFactory.CreateClient();
        var client = new CatalogsApiClient(baseClient);
        var createProductResponse = await CreateProduct(client);

        // Act
        var productByIdResponse = await client.GetProductByIdAsync(createProductResponse.Id);

        // Assert
        productByIdResponse.Should().NotBeNull();
        productByIdResponse.Product.Should().NotBeNull();
        productByIdResponse.Product.Description.Should().NotBeNull();
        productByIdResponse.Product.Name.Should().NotBeNull();
        productByIdResponse.Product.Id.Should().NotBeEmpty();
        productByIdResponse.Product.CategoryId.Should().NotBeEmpty();
        productByIdResponse.Product.Price.Should().BeGreaterThan(0);
    }

    async Task<CreateProductResponse> CreateProduct(CatalogsApiClient catalogsApiClient)
    {
        var req = new AutoFaker<CreateProductRequest>()
            .RuleFor(x => x.Price, f => f.Random.Double(10, 1000))
            .Generate();
        var createProductRequest = new CreateProductRequest(
            req.CategoryId,
            req.Description,
            req.Description,
            req.Price
        );
        var createProductResponse = await catalogsApiClient.CreateProductAsync(createProductRequest);
        return createProductResponse;
    }
}
