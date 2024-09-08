using Catalogs.ApiClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Shared.Core.Extensions;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Shared.Resiliency.Extensions;
using Shared.Resiliency.Options;
using Vertical.Slice.Template.Shared.Clients.Catalogs;
using Vertical.Slice.Template.Shared.Clients.Users;

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
        builder.Services.AddValidatedOptions<CatalogsApiClientOptions>();
        builder.Services.AddHttpClient<ICatalogsApiClient, CatalogsApiClient>(
            (client, sp) =>
            {
                var catalogApiOptions = sp.GetRequiredService<IOptions<CatalogsApiClientOptions>>();
                var policyOptions = sp.GetRequiredService<IOptions<PolicyOptions>>();
                catalogApiOptions.Value.NotBeNull();

                var baseAddress = catalogApiOptions.Value.BaseAddress;
                client.BaseAddress = new Uri(baseAddress);
                return new CatalogsApiClient(client);
            }
        );

        builder.Services.AddTransient<ICatalogsClient, CatalogsClient>();
    }
}
