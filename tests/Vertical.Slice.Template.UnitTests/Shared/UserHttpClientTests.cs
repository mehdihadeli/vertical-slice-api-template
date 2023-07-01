using AutoBogus;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute;
using RichardSzalay.MockHttp;
using Vertical.Slice.Template.Shared.Clients.Users;
using Vertical.Slice.Template.Shared.Clients.Users.Dtos;
using Vertical.Slice.Template.Shared.Core.Exceptions;
using Vertical.Slice.Template.Shared.Core.Paging;
using Vertical.Slice.Template.UnitTests.Common;
using Vertical.Slice.Template.Users.Models;

namespace Vertical.Slice.Template.UnitTests.Shared;

public class UserHttpClientTests : IClassFixture<MappingFixture>
{
    private readonly MappingFixture _mappingFixture;

    public UserHttpClientTests(MappingFixture mappingFixture)
    {
        _mappingFixture = mappingFixture;
    }

    [Fact]
    public async Task get_all_users_should_call_http_client_with_valid_parameters_once()
    {
        // Arrange
        var pageSize = 10;
        var page = 1;
        var total = 20;

        var options = Substitute.For<IOptions<UsersHttpClientOptions>>();
        var usersHttpClientOptions = new UsersHttpClientOptions
        {
            UsersEndpoint = "users",
            BaseAddress = "http://example.com"
        };
        options.Value.Returns(usersHttpClientOptions);
        var url =
            $"{
                usersHttpClientOptions.BaseAddress
            }/{
                usersHttpClientOptions.UsersEndpoint
            }*";

        var usersClient = new AutoFaker<UserClientDto>().Generate(total);

        var usersListPage = new UsersListPageClientDto
        {
            Total = total,
            Limit = pageSize,
            Skip = page,
            Users = usersClient
        };

        // https://github.com/richardszalay/mockhttp
        // https://code-maze.com/csharp-mock-httpclient-with-unit-tests/
        var mockHttp = new MockHttpMessageHandler();

        // Setup a respond for the user api (including a wildcard in the URL)
        var request = mockHttp.When(url).Respond("application/json", JsonConvert.SerializeObject(usersListPage)); // Respond with JSON

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(usersHttpClientOptions.BaseAddress);

        var pageRequest = new PageRequest { PageNumber = page, PageSize = pageSize };

        var usersHttpClient = new UsersHttpClient(client, _mappingFixture.Mapper, options);

        // Act
        await usersHttpClient.GetAllUsersAsync(pageRequest);

        // Assert
        mockHttp.GetMatchCount(request).Should().Be(1);
    }

    [Fact]
    public async Task get_all_users_should_return_users_list()
    {
        // Arrange
        var pageSize = 10;
        var page = 1;
        var total = 20;

        var options = Substitute.For<IOptions<UsersHttpClientOptions>>();
        var usersHttpClientOptions = new UsersHttpClientOptions
        {
            UsersEndpoint = "users",
            BaseAddress = "http://example.com"
        };
        options.Value.Returns(usersHttpClientOptions);
        var url =
            $"{
                usersHttpClientOptions.BaseAddress
            }/{
                usersHttpClientOptions.UsersEndpoint
            }*";

        var usersClient = new AutoFaker<UserClientDto>().Generate(total);

        var usersListPage = new UsersListPageClientDto
        {
            Total = total,
            Limit = pageSize,
            Skip = page,
            Users = usersClient
        };

        // https://github.com/richardszalay/mockhttp
        // https://code-maze.com/csharp-mock-httpclient-with-unit-tests/
        var mockHttp = new MockHttpMessageHandler();

        // Setup a respond for the user api (including a wildcard in the URL)
        var request = mockHttp.When(url).Respond("application/json", JsonConvert.SerializeObject(usersListPage)); // Respond with JSON

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(usersHttpClientOptions.BaseAddress);

        var pageRequest = new PageRequest { PageNumber = page, PageSize = pageSize };

        var users = _mappingFixture.Mapper.Map<IList<User>>(usersClient);

        var expectedPageList = new PageList<User>(users.ToList(), page, pageSize, total);

        var usersHttpClient = new UsersHttpClient(client, _mappingFixture.Mapper, options);

        // Act
        var result = await usersHttpClient.GetAllUsersAsync(pageRequest);

        // Assert
        result.Should().BeEquivalentTo(expectedPageList, c => c.ExcludingMissingMembers());
    }

    [Fact]
    public async Task get_all_users_with_http_response_exception_should_throw_http_response_exception()
    {
        // Arrange
        var pageSize = 10;
        var page = 1;
        var total = 20;

        var options = Substitute.For<IOptions<UsersHttpClientOptions>>();
        var usersHttpClientOptions = new UsersHttpClientOptions
        {
            UsersEndpoint = "users",
            BaseAddress = "http://example.com"
        };
        options.Value.Returns(usersHttpClientOptions);

        // https://github.com/richardszalay/mockhttp
        // https://code-maze.com/csharp-mock-httpclient-with-unit-tests/
        var mockHttp = new MockHttpMessageHandler();

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(usersHttpClientOptions.BaseAddress);

        var pageRequest = new PageRequest { PageNumber = page, PageSize = pageSize };

        var usersHttpClient = new UsersHttpClient(client, _mappingFixture.Mapper, options);

        // Act
        Func<Task> act = () => usersHttpClient.GetAllUsersAsync(pageRequest);

        // Assert
        await act.Should().ThrowAsync<HttpResponseException>();
    }

    [Fact]
    public async Task get_all_users_with_exception_should_throw_exception()
    {
        // Arrange
        var pageSize = 10;
        var page = 1;
        var total = 20;

        var options = Substitute.For<IOptions<UsersHttpClientOptions>>();
        var usersHttpClientOptions = new UsersHttpClientOptions
        {
            UsersEndpoint = "users",
            BaseAddress = "http://example.com"
        };
        options.Value.Returns(usersHttpClientOptions);
        var url =
            $"{
                usersHttpClientOptions.BaseAddress
            }/{
                usersHttpClientOptions.UsersEndpoint
            }*";

        // https://github.com/richardszalay/mockhttp
        // https://code-maze.com/csharp-mock-httpclient-with-unit-tests/
        var mockHttp = new MockHttpMessageHandler();

        // Setup a respond for the user api (including a wildcard in the URL)
        var request = mockHttp.When(url).Respond("application/json", JsonConvert.SerializeObject(null)); // Respond with JSON

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri(usersHttpClientOptions.BaseAddress);

        var pageRequest = new PageRequest { PageNumber = page, PageSize = pageSize };

        var usersHttpClient = new UsersHttpClient(client, _mappingFixture.Mapper, options);

        // Act
        Func<Task> act = () => usersHttpClient.GetAllUsersAsync(pageRequest);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}
