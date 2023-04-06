using Ardalis.GuardClauses;
using Catalogs.ApiClient.Catalogs;
using Microsoft.Extensions.Options;
using Shared.Web;
using Shared.Web.Extensions.ServiceCollection;

namespace Catalogs.ApiClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomHttpClients(this IServiceCollection services)
    {
        services.AddValidatedOptions<PolicyOptions>();
        services.AddValidatedOptions<ApiClientOptions>();
        services.AddHttpClient();
        services.AddTransient<ICatalogsService, CatalogsService>();

        services.AddTransient<ICatalogsApiClient, CatalogsApiClient>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var apiOptions = sp.GetRequiredService<IOptions<ApiClientOptions>>();
            Guard.Against.Null(apiOptions.Value);

            httpClient.BaseAddress = new Uri(apiOptions.Value.CatalogBaseApiAddress);
            return new CatalogsApiClient(httpClient);
        });

        return services;
    }
}