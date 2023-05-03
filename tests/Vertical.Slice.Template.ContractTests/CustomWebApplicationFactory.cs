using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Vertical.Slice.Template.Api;

namespace ApiClient.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<CatalogsApiMetadata>, IAsyncLifetime
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("test");

        builder.ConfigureWebHost(wb =>
        {
            wb.ConfigureTestServices(services => { });

            wb.ConfigureAppConfiguration((hostingContext, configurationBuilder) => { });
        });

        return base.CreateHost(builder);
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
