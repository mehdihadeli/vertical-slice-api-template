namespace Shared.Abstractions.Persistence;

public interface IDataSeeder
{
    Task SeedAllAsync(CancellationToken cancellationToken);
    int Order { get; }
}
