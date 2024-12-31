using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Polly.Wrap;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.ConnectedServiceClients.Catalogs;
using Vertical.Slice.Template.KiotaClients.Catalogs;
using Vertical.Slice.Template.Shared.Clients;
using Vertical.Slice.Template.Shared.Clients.Rest;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ConnectedServiceClientsTests;

[Collection(IntegrationTestCatalogsCollection.Name)]
public class CatalogsTestBase(
    SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
    ITestOutputHelper outputHelper
) : IntegrationTestBase<CatalogsApiMetadata, CatalogsDbContext>(sharedFixture, outputHelper)
{
    private ICatalogsConnectedServiceClient? _catalogsConnectedServiceClient;
    private ICatalogsKiotaClient? _catalogsKiotaClient;
    private ICatalogsRestClient? _catalogsRestClient;
    public ICatalogsConnectedServiceClient CatalogsConnectedServiceClient =>
        _catalogsConnectedServiceClient ??=
            SharedFixture.ServiceProvider.GetRequiredService<ICatalogsConnectedServiceClient>();

    public ICatalogsKiotaClient CatalogsKiotaClient =>
        _catalogsKiotaClient ??= SharedFixture.ServiceProvider.GetRequiredService<ICatalogsKiotaClient>();
    public ICatalogsRestClient CatalogsRestClient =>
        _catalogsRestClient ??= SharedFixture.ServiceProvider.GetRequiredService<ICatalogsRestClient>();

    protected override void SetupTestConfigureServices(IServiceCollection services)
    {
        // replace existing app clients with WebApplicationFactory client.
        services.ReplaceTransient<ICatalogsConectedServiceApiClient>(sp => new CatalogsConectedServiceApiClient(
            SharedFixture.GuestClient
        ));
        services.ReplaceTransient<ICatalogsRestClient>(sp => new CatalogsRestClient(
            SharedFixture.GuestClient,
            sp.GetRequiredService<AsyncPolicyWrap>(),
            sp.GetRequiredService<IOptions<CatalogsRestClientOptions>>()
        ));

        services.ReplaceTransient<ICatalogsConectedServiceApiClient>(sp => new CatalogsConectedServiceApiClient(
            SharedFixture.GuestClient
        ));

        services.Remove(services.FirstOrDefault(d => d.ServiceType == typeof(CatalogsKiotaApiClient)));
        services
            .AddHttpClient<CatalogsKiotaApiClient>()
            .AddTypedClient(
                (httpClient, sp) =>
                {
                    // https://learn.microsoft.com/en-us/openapi/kiota/quickstarts/dotnet#create-the-client-application
                    var authenticationProvider = new AnonymousAuthenticationProvider();

                    var requestAdapter = new HttpClientRequestAdapter(
                        authenticationProvider,
                        httpClient: SharedFixture.GuestClient
                    );

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
            });

        base.SetupTestConfigureServices(services);
    }
}
