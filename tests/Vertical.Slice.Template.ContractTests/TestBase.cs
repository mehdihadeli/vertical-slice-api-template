using Microsoft.Extensions.DependencyInjection;
using Vertical.Slice.Template.Shared.Data;

namespace ApiClient.Tests;

[Collection(SharedTestCollection.Name)]
public class TestBase : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _appFactory;
    private CatalogsDbContext? _catalogsDbContext;

    public TestBase(CustomWebApplicationFactory appFactory)
    {
        _appFactory = appFactory;
    }

    public CustomWebApplicationFactory Factory => _appFactory;
    public AsyncServiceScope ServiceScope { get; private set; }
    public CatalogsDbContext CatalogsDbContext =>
        _catalogsDbContext ??= ServiceScope.ServiceProvider.GetRequiredService<CatalogsDbContext>();

    public async Task ResetDatabaseAsync(CancellationToken cancellationToken)
    {
        await CatalogsDbContext.Database.EnsureDeletedAsync(cancellationToken);
        await CatalogsDbContext.Database.EnsureCreatedAsync(cancellationToken);
    }

    public async Task InitializeAsync()
    {
        ServiceScope = _appFactory.Services.CreateAsyncScope();
        await ResetDatabaseAsync(CancellationToken.None);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
