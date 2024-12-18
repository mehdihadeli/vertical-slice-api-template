namespace Shared.Abstractions.Persistence;

public interface IMigrationSchema
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
