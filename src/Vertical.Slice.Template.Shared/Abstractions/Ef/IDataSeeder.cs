namespace Vertical.Slice.Template.Shared.Abstractions.Ef;

public interface IDataSeeder
{
    Task SeedAllAsync(CancellationToken cancellationToken);
    int Order { get; }
}
