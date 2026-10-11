using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Shared.Web;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.TestsShared.XunitCategories;

namespace Vertical.Slice.Template.DependencyTests;

public class DependencyTests
{
    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public void validate_service_dependencies()
    {
        var factory = new WebApplicationFactory<CatalogsApiMetadata>().WithWebHostBuilder(webHostBuilder =>
        {
            webHostBuilder.UseEnvironment(Environments.DependencyTest);

            webHostBuilder.ConfigureTestServices(services =>
            {
                services.AddTransient<IServiceCollection>(_ => services);
            });
        });

        using var scope = factory.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var services = sp.GetRequiredService<IServiceCollection>();
        sp.ValidateDependencies(services, typeof(CatalogsApiMetadata).Assembly, typeof(CatalogsMetadata).Assembly);
    }
}
