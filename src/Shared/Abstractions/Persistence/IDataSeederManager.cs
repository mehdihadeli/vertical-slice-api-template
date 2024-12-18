namespace Shared.Abstractions.Persistence;

public interface IDataSeederManager
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
