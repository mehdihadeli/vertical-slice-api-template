using AutoBogus;
using Catalogs.ApiClient.Catalogs.Dtos;
using FluentAssertions;

namespace ApiClient.Tests.Catalogs;

public class CatalogsServiceTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _appFactory;

    public CatalogsServiceTests(CustomWebApplicationFactory appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task create_product_can_create_product_with_valid_data()
    {
        var catalogsService = _appFactory.CatalogsService;
        var createProduct = new AutoFaker<CreateProductInput>().Generate();
        var response = await catalogsService.CreateProductAsync(createProduct, CancellationToken.None);
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();
    }
}
