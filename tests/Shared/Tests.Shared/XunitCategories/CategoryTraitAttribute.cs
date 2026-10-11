namespace Vertical.Slice.Template.TestsShared.XunitCategories;

// Ref: https://dateo-software.de/blog/test-categories-in-xunit
// https://www.brendanconnolly.net/organizing-tests-with-xunit-traits/
// https://github.com/xunit/samples.xunit/tree/main/TraitExtensibility

/// <summary>
/// Could filter by 'dotnet test --filter "Category=TestCategory"' in running tests in command line
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class CategoryTraitAttribute : Attribute
{
    public CategoryTraitAttribute(TestCategory category) => Name = category;

    public TestCategory Name { get; }
    public string Category => Name.ToString();
}

public enum TestCategory
{
    Unit,
    Integration,
    EndToEnd,
    LoadTest,
    SkipCI,
}
