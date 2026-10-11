using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Persistence;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.Catalogs.TestShared;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;

namespace Vertical.Slice.Template.ContractTests;

public class CatalogsIntegrationTestBase(
    SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
    ITestOutputHelper outputHelper
)
    : IntegrationTestBase<CatalogsApiMetadata, CatalogsDbContext>(sharedFixture, outputHelper),
        IClassFixture<SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext>>
{
    protected override void SetupTestConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, CatalogsTestDataSeeder>();
    }
}
