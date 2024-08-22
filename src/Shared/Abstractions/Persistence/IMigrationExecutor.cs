namespace Shared.Abstractions.Persistence;

public interface IMigrationExecutor
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
