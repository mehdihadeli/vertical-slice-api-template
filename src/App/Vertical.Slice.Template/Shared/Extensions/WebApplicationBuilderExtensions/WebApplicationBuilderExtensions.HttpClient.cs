using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Shared.Core.Extensions;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Shared.Resiliency;
using Shared.Resiliency.HttpClient;
using Shared.Resiliency.Options;
using Vertical.Slice.Template.ConnectedServiceClients.Catalogs;
using Vertical.Slice.Template.KiotaClients.Catalogs;
using Vertical.Slice.Template.Shared.Clients;
using Vertical.Slice.Template.Shared.Clients.ConnectedService;
using Vertical.Slice.Template.Shared.Clients.Kiota;
using Vertical.Slice.Template.Shared.Clients.Rest;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCustomHttpClients(this WebApplicationBuilder builder)
    {
        AddCatalogsApiClient(builder);

        return builder;
    }

    private static void AddCatalogsApiClient(this WebApplicationBuilder builder)
    {
        AddCatalogsConnectedServiceClient(builder);

        AddCatalogsRestClient(builder);

        AddCatalogsKiotaClient(builder);
    }

    private static void AddCatalogsRestClient(WebApplicationBuilder builder)
    {
        // rest client
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#typed-clients
        builder.Services.AddValidatedOptions<CatalogsRestClientOptions>();
        builder.Services.AddCustomHttpClient<ICatalogsRestClient, CatalogsRestClient, CatalogsRestClientOptions>();
    }

    private static void AddCatalogsConnectedServiceClient(WebApplicationBuilder builder)
    {
        // connected service client
        builder.Services.AddValidatedOptions<CatalogsConnectedServiceClientOptions>();

        builder
            .Services.AddHttpClient<ICatalogsConectedServiceApiClient, CatalogsConectedServiceApiClient>(
                (sp, client) =>
                {
                    var connectedServiceClientOptions = sp.GetRequiredService<
                            IOptions<CatalogsConnectedServiceClientOptions>
                        >()
                        .Value.NotBeNull();
                    var policyOptions = sp.GetRequiredService<IOptions<PolicyOptions>>().Value.NotBeNull();

                    var baseAddress = connectedServiceClientOptions.BaseAddress;
                    client.Timeout = TimeSpan.FromSeconds(policyOptions.TimeoutPolicyOptions.TimeoutInSeconds);
                    client.BaseAddress = new Uri(baseAddress);
                }
            )
            .ConfigureStandardResilienceHandler();

        builder.Services.AddTransient<ICatalogsConnectedServiceClient, CatalogsConnectedServiceClient>();
    }

    private static void AddCatalogsKiotaClient(WebApplicationBuilder builder)
    {
        builder.Services.AddValidatedOptions<CatalogsKiotaClientOptions>();

        // https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
        builder
            .Services.AddHttpClient<CatalogsKiotaApiClient>()
            .AddTypedClient<CatalogsKiotaApiClient>(
                (httpClient, sp) =>
                {
                    // https://learn.microsoft.com/en-us/openapi/kiota/quickstarts/dotnet#create-the-client-application
                    var kiotaClientOptions = sp.GetRequiredService<IOptions<CatalogsKiotaClientOptions>>()
                        .Value.NotBeNull();
                    var policyOptions = sp.GetRequiredService<IOptions<PolicyOptions>>().Value.NotBeNull();

                    var baseAddress = kiotaClientOptions.BaseAddress;
                    var authenticationProvider = new AnonymousAuthenticationProvider();
                    httpClient.BaseAddress = new Uri(baseAddress);
                    httpClient.Timeout = TimeSpan.FromSeconds(policyOptions.TimeoutPolicyOptions.TimeoutInSeconds);

                    var requestAdapter = new HttpClientRequestAdapter(authenticationProvider, httpClient: httpClient);

                    return new CatalogsKiotaApiClient(requestAdapter);
                }
            )
            .ConfigurePrimaryHttpMessageHandler(_ =>
            {
                // adding some default handlers which exists for kiota
                IList<DelegatingHandler> defaultHandlers = KiotaClientFactory.CreateDefaultHandlers();

                HttpMessageHandler defaultHttpMessageHandler = KiotaClientFactory.GetDefaultHttpMessageHandler();

                return KiotaClientFactory.ChainHandlersCollectionAndGetFirstLink(
                    defaultHttpMessageHandler,
                    [.. defaultHandlers]
                )!;
            })
            .ConfigureStandardResilienceHandler();

        builder.Services.AddTransient<ICatalogsKiotaClient, CatalogsKiotaClient>();
    }
}
