using ApiClient.Catalogs;
using Catalogs.ApiClient;
using Microsoft.Extensions.Options;
using Shared.Core.Extensions;
using Shared.Web;
using Shared.Web.Extensions.ServiceCollection;

namespace ApiClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomHttpClients(this IServiceCollection services)
    {
        services.AddValidatedOptions<PolicyOptions>();
        services.AddHttpClient();

        AddCatalogsApiClient(services);

        return services;
    }

    private static void AddCatalogsApiClient(IServiceCollection services)
    {
        services.AddValidatedOptions<CatalogsApiClientOptions>();
        services.AddTransient<ICatalogsClient, CatalogsClient>();

        services.AddTransient<ICatalogsApiClient, CatalogsApiClient>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var apiOptions = sp.GetRequiredService<IOptions<CatalogsApiClientOptions>>();
            apiOptions.Value.NotBeNull();

            httpClient.BaseAddress = new Uri(apiOptions.Value.CatalogBaseApiAddress);
            return new CatalogsApiClient(httpClient);
        });
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(x =>
        {
            x.AddProfile<ClientsMappingProfile>();
        });
        return services;
    }
}
