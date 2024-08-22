namespace Shared.Abstractions.Persistence.Ef;

public interface IDataSeeder
{
    Task SeedAllAsync(CancellationToken cancellationToken);
    int Order { get; }
}
