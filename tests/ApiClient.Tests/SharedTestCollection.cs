namespace ApiClient.Tests;

// https://stackoverflow.com/questions/43082094/use-multiple-collectionfixture-on-my-test-class-in-xunit-2-x
[CollectionDefinition(Name)]
public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string Name = "Shared Test Collection";
}
