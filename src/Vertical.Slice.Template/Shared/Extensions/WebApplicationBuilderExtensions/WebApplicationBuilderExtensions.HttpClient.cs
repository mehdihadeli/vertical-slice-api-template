using Catalogs.ApiClient;
using Microsoft.AspNetCore.Builder;
using Vertical.Slice.Template.Shared.Clients.Catalogs;
using Vertical.Slice.Template.Shared.Clients.Users;
using Vertical.Slice.Template.Shared.Resiliency.Extensions;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCustomHttpClients(this WebApplicationBuilder builder)
    {
        builder.Services.AddCustomPolicyRegistry();

        AddCatalogsApiClient(builder);
        AddUsersHttpClient(builder);

        return builder;
    }

    private static void AddUsersHttpClient(this WebApplicationBuilder builder)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#typed-clients
        builder.Services.AddCustomHttpClient<IUsersHttpClient, UsersHttpClient, UsersHttpClientOptions>();
    }

    private static void AddCatalogsApiClient(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICatalogsClient, CatalogsClient>();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#named-clients
        // Add custom `http client` for using in CatalogsApiClient
        builder.Services.AddCustomHttpClient<CatalogsApiClientOptions>(CatalogsClient.ClientName);

        builder.Services.AddTransient<ICatalogsApiClient, CatalogsApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#createclient
            var httpClient = factory.CreateClient(CatalogsClient.ClientName);
            return new CatalogsApiClient(httpClient);
        });
    }
}
