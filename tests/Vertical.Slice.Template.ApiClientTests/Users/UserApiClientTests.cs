using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Paging;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Clients.Users;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ApiClientTests.Users;

[Collection(IntegrationTestUsersCollection.Name)]
public class UserApiClientTests(SharedFixture<CatalogsApiMetadata> sharedFixture, ITestOutputHelper outputHelper)
    : IntegrationTest<CatalogsApiMetadata>(sharedFixture, outputHelper)
{
    private IUsersHttpClient? _usersHttpClient;

    public IUsersHttpClient UsersClient =>
        _usersHttpClient ??= SharedFixture.ServiceProvider.GetRequiredService<IUsersHttpClient>();

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task get_users_can_get_users_with_valid_data()
    {
        // Act
        var response = await UsersClient.GetAllUsersAsync(
            new PageRequest { PageNumber = 1, PageSize = 10 },
            CancellationToken.None
        );

        // Assert
        response.Should().NotBeNull();
        response.Items.Should().HaveCountGreaterThan(0);
    }
}
