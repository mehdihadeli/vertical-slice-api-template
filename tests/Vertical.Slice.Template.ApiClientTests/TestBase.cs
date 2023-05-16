using Catalogs.ApiClient;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.ApiClient.Catalogs;
using Vertical.Slice.Template.ApiClient.Extensions;
using Vertical.Slice.Template.ApiClient.RickAndMorty;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ApiClientTests;

[Collection(SharedTestCollection.Name)]
public class TestBase : IntegrationTestBase<CatalogsApiMetadata, CatalogsDbContext>
{
    private ICatalogsClient? _catalogsService;
    private IRickAndMortyClient? _rickAndMortyClient;
    public ICatalogsClient CatalogsClient =>
        _catalogsService ??= SharedFixture.ServiceProvider.GetRequiredService<ICatalogsClient>();

    public IRickAndMortyClient RickAndMortyClient =>
        _rickAndMortyClient ??= SharedFixture.ServiceProvider.GetRequiredService<IRickAndMortyClient>();

    public TestBase(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper)
    {
        SharedFixture.Factory = SharedFixture.Factory.WithWebHostBuilder(wb =>
        {
            wb.ConfigureTestServices(services =>
            {
                services.AddCustomHttpClients();
                services.AddMappings();
                services.AddTransient<ICatalogsApiClient>(x => new CatalogsApiClient(SharedFixture.GuestClient));
            });

            wb.ConfigureAppConfiguration((hostingContext, configurationBuilder) => { });
        });
    }
}
