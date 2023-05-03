using Microsoft.Extensions.DependencyInjection;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class CatalogsIntegrationTestBase : IntegrationTestBase<CatalogsApiMetadata, CatalogsDbContext>
{
    public CatalogsIntegrationTestBase(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    protected override void RegisterTestConfigureServices(IServiceCollection services)
    {
        //// here we use same data seeder of service but if we need different data seeder for test for can replace it
    }
}
