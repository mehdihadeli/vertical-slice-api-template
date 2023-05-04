using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Vertical.Slice.Template.Api;

namespace Vertical.Slice.Template.TestsShared.Factory;

public class CustomWebApplicationFactory : WebApplicationFactory<CatalogsApiMetadata>, IAsyncLifetime
{
    private readonly Action<IWebHostBuilder>? _webHostBuilder;

    public CustomWebApplicationFactory(Action<IWebHostBuilder>? webHostBuilder = null)
    {
        _webHostBuilder = webHostBuilder;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("test");

        // builder.ConfigureWebHost(wb =>
        // {
        //     wb.ConfigureTestServices(services => { });
        //
        //     wb.ConfigureAppConfiguration((hostingContext, configurationBuilder) => { });
        // });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _webHostBuilder?.Invoke(builder);

        builder.ConfigureTestServices(services => { });

        builder.ConfigureAppConfiguration((hostingContext, configurationBuilder) => { });

        base.ConfigureWebHost(builder);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
