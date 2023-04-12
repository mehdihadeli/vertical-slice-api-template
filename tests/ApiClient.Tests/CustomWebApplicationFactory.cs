using ApiClient.Catalogs;
using ApiClient.Extensions;
using Catalogs.ApiClient;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertical.Slice.Template.Api;

namespace ApiClient.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private ICatalogsService? _catalogsService;
    public ICatalogsService CatalogsService => _catalogsService ??= Services.GetRequiredService<ICatalogsService>();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureWebHost(wb =>
        {
            wb.ConfigureTestServices(services =>
            {
                services.AddCustomHttpClients();
                services.AddMappings();
                services.AddTransient<ICatalogsApiClient>(x => new CatalogsApiClient(CreateClient()));
            });

            wb.ConfigureAppConfiguration((hostingContext, configurationBuilder) => { });
        });

        return base.CreateHost(builder);
    }
}
