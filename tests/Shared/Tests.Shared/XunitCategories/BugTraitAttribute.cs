namespace Vertical.Slice.Template.TestsShared.XunitCategories;

/// <summary>
/// Could filter by `dotnet test --filter "Category=Bug"` and `dotnet test --filter "Bug=201"` in running tests in command line
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class BugTraitAttribute(int id) : Attribute
{
    public int Id { get; } = id;
    public int Bug => Id;
    public string Category => "Bug";
}
