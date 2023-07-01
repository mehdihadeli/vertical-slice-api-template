namespace Vertical.Slice.Template.Shared.Abstractions.Ef;

public interface IMigrationExecutor
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
