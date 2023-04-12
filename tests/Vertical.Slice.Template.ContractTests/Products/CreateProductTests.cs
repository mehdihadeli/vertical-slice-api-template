using ApiClient.Tests;
using AutoBogus;
using Catalogs.ApiClient;
using FluentAssertions;

namespace Vertical.Slice.Template.ContractTests.Products;

public class CreateProductTests : TestBase
{
    public CreateProductTests(CustomWebApplicationFactory appFactory)
        : base(appFactory) { }

    [Fact]
    public async Task BasicContractTest()
    {
        using var baseClient = Factory.CreateClient();
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
