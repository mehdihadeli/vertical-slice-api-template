namespace Vertical.Slice.Template.TestsShared.XunitCategories;

/// <summary>
/// Could filter by `dotnet test --filter "Category=Feature"` and `dotnet test --filter "Feature=201"` in running tests in command line
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class FeatureTraitAttribute(int id) : Attribute
{
    public int Id { get; } = id;
    public int Feature => Id;
    public string Category => "Feature";
}
