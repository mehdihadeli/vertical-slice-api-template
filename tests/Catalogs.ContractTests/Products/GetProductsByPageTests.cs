using Catalogs.ApiClient;
using Catalogs.Products.Features.GettingProductsByPage;
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

        await client.GetProductByPageAsync(pageSize: 10, pageNumber: 1, filters: null, sortOrder: null);
    }
}
