namespace Vertical.Slice.Template.Shared.Abstractions.Ef;

public interface IDataSeeder
{
    Task SeedAllAsync();
    int Order { get; }
}
