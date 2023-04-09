using AutoBogus;
using Catalogs.ApiClient;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Vertical.Slice.Template.Api;

namespace Vertical.Slice.Template.ContractTests.Products;

public class CreateProductTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _appFactory;

    public CreateProductTests(WebApplicationFactory<Program> appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task BasicContractTest()
    {
        using var baseClient = _appFactory.CreateClient();
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
