using Shared.Abstractions.Persistence.Ef;

namespace Vertical.Slice.Template.IntegrationTests;

public class CatalogsTestDataSeeder : ITestDataSeeder
{
    public Task SeedAllAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public int Order => 1;
}
