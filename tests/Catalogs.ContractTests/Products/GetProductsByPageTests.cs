using Catalogs.ApiClient;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Catalogs.ContractTests.Products;

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

        var response = await client.GetProductByPageAsync(pageSize: 10, pageNumber: 1, filters: null, sortOrder: null);
        response.Should().NotBeNull();
        response.Products.Should().NotBeNull();
        response.Products.Items.Should().NotBeNull();
        response.Products.TotalCount.Should().Be(0);
        response.Products.TotalPages.Should().Be(0);
        response.Products.PageSize.Should().Be(10);
        response.Products.PageNumber.Should().Be(1);
    }
}
