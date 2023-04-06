using Bogus;
using Catalogs.ApiClient;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Catalogs.ContractTests.Products;

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

        // Act
        Func<Task> act = async () => await client.GetProductByIdAsync(new Faker().Random.Guid());

        // Assert
        //https://fluentassertions.com/exceptions/
        var res = await act.Should().ThrowAsync<ApiException>();
        var exception = res.Which;
        exception.Response.Should().NotBeNull();
        var problem = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemDetails>(exception.Response!);
        problem.Should().NotBeNull();
        problem!.Title.Should().Contain("NotFoundException");
    }
}
