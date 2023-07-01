using Catalogs.ApiClient;
using Microsoft.Extensions.DependencyInjection;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Clients.Catalogs;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ApiClientTests;

[Collection(IntegrationTestCatalogsCollection.Name)]
public class CatalogsTestBase : IntegrationTestBase<CatalogsApiMetadata, CatalogsDbContext>
{
    private ICatalogsClient? _catalogsClient;
    public ICatalogsClient CatalogsClient =>
        _catalogsClient ??= SharedFixture.ServiceProvider.GetRequiredService<ICatalogsClient>();

    public CatalogsTestBase(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    protected override void RegisterTestConfigureServices(IServiceCollection services)
    {
        services.AddTransient<ICatalogsApiClient>(x => new CatalogsApiClient(SharedFixture.GuestClient));
        base.RegisterTestConfigureServices(services);
    }
}
