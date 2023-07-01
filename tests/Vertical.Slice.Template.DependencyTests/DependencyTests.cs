using Microsoft.Extensions.DependencyInjection;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Core.Extensions.ServiceCollectionsExtensions;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;

namespace Vertical.Slice.Template.DependencyTests;

public class DependencyTests : IClassFixture<SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext>>
{
    private readonly SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> _sharedFixture;

    public DependencyTests(SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture)
    {
        _sharedFixture = sharedFixture;
        sharedFixture.ConfigureTestServices(services =>
        {
            services.AddTransient<IServiceCollection>(_ => services);
        });
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public void validate_service_dependencies()
    {
        using var scope = _sharedFixture.Factory.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var services = sp.GetRequiredService<IServiceCollection>();
        sp.ValidateDependencies(services, typeof(CatalogsApiMetadata).Assembly, typeof(CatalogsMetadata).Assembly);
    }
}
