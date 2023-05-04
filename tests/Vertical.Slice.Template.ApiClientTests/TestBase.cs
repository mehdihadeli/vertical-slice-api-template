using ApiClient.Catalogs;
using ApiClient.Extensions;
using Catalogs.ApiClient;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ApiClientTests;

[Collection(SharedTestCollection.Name)]
public class TestBase : IntegrationTestBase<CatalogsApiMetadata, CatalogsDbContext>
{
    private ICatalogsService? _catalogsService;
    public ICatalogsService CatalogsService =>
        _catalogsService ??= SharedFixture.ServiceProvider.GetRequiredService<ICatalogsService>();

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
