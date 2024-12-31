using FluentValidation;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Wrap;
using Shared.Core.Paging;
using Sieve.Models;
using Vertical.Slice.Template.TestsShared.XunitCategories;

namespace Vertical.Slice.Template.UnitTests.Common;

[CollectionDefinition(nameof(QueryTestCollection))]
public class QueryTestCollection : ICollectionFixture<CatalogsUnitTestBase>;

//https://stackoverflow.com/questions/43082094/use-multiple-collectionfixture-on-my-test-class-in-xunit-2-x
// note: each class could have only one collection
[Collection(UnitTestCollection.Name)]
[CategoryTrait(TestCategory.Unit)]
public class CatalogsUnitTestBase : IAsyncDisposable
{
    public AsyncPolicyWrap CombinedPolicy { get; }
    public ApplicationSieveProcessor SieveProcessor { get; }

    // We don't need to inject `CustomersServiceMockServersFixture` class fixture in the constructor because it initialized by `collection fixture` and its static properties are accessible in the codes
    public CatalogsUnitTestBase()
    {
        IOptions<SieveOptions> options = Options.Create(new SieveOptions { DefaultPageSize = 10, MaxPageSize = 10 });
        SieveProcessor = new ApplicationSieveProcessor(options);

        var retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(1);
        var timeoutPolicy = Policy.TimeoutAsync(30);
        CombinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);
    }

    public IValidator<T> GetFakeValidator<T>()
        where T : class
    {
        return new FakeValidator<T>();
    }

    public async ValueTask DisposeAsync() { }
}
