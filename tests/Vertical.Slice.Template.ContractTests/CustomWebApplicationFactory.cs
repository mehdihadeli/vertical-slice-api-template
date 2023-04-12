using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Vertical.Slice.Template.Api;

namespace ApiClient.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureWebHost(wb =>
        {
            wb.ConfigureTestServices(services => { });

            wb.ConfigureAppConfiguration((hostingContext, configurationBuilder) => { });
        });

        return base.CreateHost(builder);
    }
}
