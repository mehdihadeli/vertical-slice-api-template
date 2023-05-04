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

public class CreateProductTests : CatalogsIntegrationTestBase
{
    public CreateProductTests(
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

        var req = new AutoFaker<CreateProductRequest>().Generate();
        var createProductRequest = new CreateProductRequest(
            req.CategoryId,
            req.Description,
            req.Description,
            req.Price
        );

        var response = await client.CreateProductAsync(createProductRequest);
        response.Id.Should().NotBeEmpty();
    }
}
