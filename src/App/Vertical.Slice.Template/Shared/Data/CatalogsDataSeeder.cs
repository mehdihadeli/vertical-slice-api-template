using Shared.Abstractions.Persistence;
using Shared.Abstractions.Persistence.Ef;

namespace Vertical.Slice.Template.Shared.Data;

public class CatalogsDataSeeder : IDataSeeder
{
    public Task SeedAllAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public int Order => 1;
}
