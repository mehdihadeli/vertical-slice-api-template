using AutoBogus;
using Catalogs.ApiClient;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Vertical.Slice.Template.Api;

namespace Vertical.Slice.Template.ContractTests.Products;

public class GetProductsByPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _appFactory;

    public GetProductsByPageTests(WebApplicationFactory<Program> appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task BasicContractTest()
    {
        using var baseClient = _appFactory.CreateClient();
        var client = new CatalogsApiClient(baseClient);
        var createdProduct = await CreateProduct(client);

        var response = await client.GetProductsByPageAsync(pageSize: 10, pageNumber: 1, filters: null, sortOrder: null);
        response.Should().NotBeNull();
        response.Products.Should().NotBeNull();
        response.Products.Items.Should().NotBeNull();
        response.Products.TotalCount.Should().Be(1);
        response.Products.TotalPages.Should().Be(1);
        response.Products.PageSize.Should().Be(10);
        response.Products.PageNumber.Should().Be(1);
        response.Products.Items.First().Should().NotBeNull();
        response.Products.Items.First().Description.Should().NotBeNull();
        response.Products.Items.First().Name.Should().NotBeNull();
        response.Products.Items.First().Id.Should().NotBeEmpty();
        response.Products.Items.First().CategoryId.Should().NotBeEmpty();
        response.Products.Items.First().Price.Should().BeGreaterThan(0);
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
