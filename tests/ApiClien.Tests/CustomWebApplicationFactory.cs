using ApiClient.Catalogs;
using Catalogs.ApiClient;
using Catalogs.ApiClient.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                services.AddTransient<ICatalogsApiClient>(x =>
                {
                    // replace our CatalogsApiClient, internal httpclient with factory httpclient
                    return new CatalogsApiClient(CreateClient());
                });
            });

            wb.ConfigureAppConfiguration((hostingContext, configurationBuilder) => { });
        });

        return base.CreateHost(builder);
    }
}
