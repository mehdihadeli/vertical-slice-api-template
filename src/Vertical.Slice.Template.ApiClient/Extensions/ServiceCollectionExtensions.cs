using Catalogs.ApiClient;
using Vertical.Slice.Template.ApiClient.Catalogs;
using Vertical.Slice.Template.ApiClient.RickAndMorty;
using Vertical.Slice.Template.Shared.Resiliency.Extensions;

namespace Vertical.Slice.Template.ApiClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomHttpClients(this IServiceCollection services)
    {
        services.AddCustomPolicyRegistry();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#basic-usage
        // Register IHttpClientFactory by calling AddHttpClient
        services.AddHttpClient();

        AddCatalogsApiClient(services);
        AddRikAndMortyClient(services);

        return services;
    }

    private static void AddRikAndMortyClient(IServiceCollection services)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#typed-clients
        services.AddCustomHttpClient<IRickAndMortyClient, RickAndMortyClient, RikAndMortyApiClientOptions>();
    }

    private static void AddCatalogsApiClient(IServiceCollection services)
    {
        services.AddTransient<ICatalogsClient, CatalogsClient>();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#named-clients
        // Add custom `http client` for using in CatalogsApiClient
        services.AddCustomHttpClient<CatalogsApiClientOptions>(CatalogsClient.ClientName);

        services.AddTransient<ICatalogsApiClient, CatalogsApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#createclient
            var httpClient = factory.CreateClient(CatalogsClient.ClientName);
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
