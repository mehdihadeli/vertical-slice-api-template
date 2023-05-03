using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Shared.Web.Extensions.ServiceCollection;
using Vertical.Slice.Template.Api;

namespace Vertical.Slice.Template.DependencyTests;

public class DependencyTests
{
    [Fact]
    public void validate_service_dependencies()
    {
        var factory = new WebApplicationFactory<CatalogsApiMetadata>().WithWebHostBuilder(webHostBuilder =>
        {
            webHostBuilder.UseEnvironment("test");

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
