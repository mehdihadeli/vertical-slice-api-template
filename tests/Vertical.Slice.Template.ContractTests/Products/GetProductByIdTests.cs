using ApiClient.Tests;
using AutoBogus;
using Catalogs.ApiClient;
using FluentAssertions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ContractTests.Products;

public class GetProductByIdTests : CatalogsIntegrationTestBase
{
    public GetProductByIdTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task BasicContractTest()
    {
        using var baseClient = SharedFixture.GuestClient;
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
