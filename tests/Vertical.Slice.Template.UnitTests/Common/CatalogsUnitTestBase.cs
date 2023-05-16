using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Vertical.Slice.Template.Shared.Core.Paging;
using Vertical.Slice.Template.TestsShared.XunitCategories;

namespace Vertical.Slice.Template.UnitTests.Common;

[CollectionDefinition(nameof(QueryTestCollection))]
public class QueryTestCollection : ICollectionFixture<CatalogsUnitTestBase> { }

//https://stackoverflow.com/questions/43082094/use-multiple-collectionfixture-on-my-test-class-in-xunit-2-x
// note: each class could have only one collection
[Collection(UnitTestCollection.Name)]
[CategoryTrait(TestCategory.Unit)]
public class CatalogsUnitTestBase : IAsyncDisposable
{
    // We don't need to inject `CustomersServiceMockServersFixture` class fixture in the constructor because it initialized by `collection fixture` and its static properties are accessible in the codes
    public CatalogsUnitTestBase()
    {
        Mapper = MapperFactory.Create();
        IOptions<SieveOptions> options = Options.Create(new SieveOptions { DefaultPageSize = 10, MaxPageSize = 10 });
        SieveProcessor = new ApplicationSieveProcessor(options);
    }

    public IMapper Mapper { get; }
    public ApplicationSieveProcessor SieveProcessor { get; }

    public IValidator<T> GetFakeValidator<T>()
        where T : class
    {
        return new FakeValidator<T>();
    }

    public async ValueTask DisposeAsync() { }
}
