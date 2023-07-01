using AutoBogus;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Vertical.Slice.Template.Shared.Clients.Users;
using Vertical.Slice.Template.Shared.Core.Exceptions;
using Vertical.Slice.Template.Shared.Core.Paging;
using Vertical.Slice.Template.UnitTests.Common;
using Vertical.Slice.Template.Users.Dtos;
using Vertical.Slice.Template.Users.GetUsers;
using Vertical.Slice.Template.Users.Models;

namespace TDDSample.Tests.UnitTests.Users.Features.GetUsers;

// https://www.testwithspring.com/lesson/the-best-practices-of-nested-unit-tests/

public class GetUsersHandlerTests : IClassFixture<MappingFixture>
{
    private readonly MappingFixture _mappingFixture;

    public GetUsersHandlerTests(MappingFixture mappingFixture)
    {
        _mappingFixture = mappingFixture;
    }

    [Fact]
    public async Task handle_should_call_users_http_client_once()
    {
        var usersHttpClient = Substitute.For<IUsersHttpClient>();
        var cancellationToken = CancellationToken.None;
        var page = 1;
        var pageSize = 10;

        var users = new AutoFaker<User>().Generate(20);
        var pageResult = new PageList<User>(users, page, pageSize, users.Count);

        usersHttpClient.GetAllUsersAsync(Arg.Any<PageRequest>(), cancellationToken).Returns(pageResult);

        var handler = new GetUsersHandler(usersHttpClient, _mappingFixture.Mapper);

        var query = GetUsersByPage.Of(new PageRequest { PageNumber = page, PageSize = pageSize });
        await handler.Handle(query, cancellationToken);

        await usersHttpClient.Received(1).GetAllUsersAsync(Arg.Any<PageRequest>(), cancellationToken);
    }

    [Fact]
    public async Task handle_with_valid_request_should_returns_users()
    {
        var usersHttpClient = Substitute.For<IUsersHttpClient>();
        var cancellationToken = CancellationToken.None;
        var page = 1;
        var pageSize = 10;

        var users = new AutoFaker<User>().Generate(20);
        var pageResult = new PageList<User>(users, page, pageSize, users.Count);

        usersHttpClient.GetAllUsersAsync(Arg.Any<PageRequest>(), cancellationToken).Returns(pageResult);

        var handler = new GetUsersHandler(usersHttpClient, _mappingFixture.Mapper);

        var query = GetUsersByPage.Of(new PageRequest { PageNumber = page, PageSize = pageSize });
        var result = await handler.Handle(query, cancellationToken);

        result.Should().NotBeNull();
        var pageListDto = result.Users.As<PageList<UserDto>>();
        pageListDto.Should().NotBeNull();
        pageListDto.Should().NotBeNull();
        pageListDto.PageNumber.Should().Be(page);
        pageListDto.PageSize.Should().Be(pageSize);
        pageListDto.TotalCount.Should().Be(users.Count);
        pageListDto.Items.Should().BeEquivalentTo(pageResult.Items);
    }

    [Fact]
    public async Task handle_with_http_response_exception_should_returns_correct_error_result()
    {
        var usersHttpClient = Substitute.For<IUsersHttpClient>();
        var cancellationToken = CancellationToken.None;
        var page = 1;
        var pageSize = 10;

        usersHttpClient
            .GetAllUsersAsync(Arg.Any<PageRequest>(), cancellationToken)
            .ThrowsAsync(new HttpResponseException(404, "Not Found"));

        var handler = new GetUsersHandler(usersHttpClient, _mappingFixture.Mapper);

        var query = GetUsersByPage.Of(new PageRequest { PageNumber = page, PageSize = pageSize });

        var act = () => handler.Handle(query, cancellationToken);

        await act.Should().ThrowAsync<HttpResponseException>();
    }

    [Fact]
    public async Task handle_with_exception_should_returns_correct_error_result()
    {
        var usersHttpClient = Substitute.For<IUsersHttpClient>();
        var cancellationToken = CancellationToken.None;
        var page = 1;
        var pageSize = 10;

        usersHttpClient
            .GetAllUsersAsync(Arg.Any<PageRequest>(), cancellationToken)
            .ThrowsAsync(new Exception("Internal server error"));

        var handler = new GetUsersHandler(usersHttpClient, _mappingFixture.Mapper);

        var query = GetUsersByPage.Of(new PageRequest { PageNumber = page, PageSize = pageSize });
        var act = () => handler.Handle(query, cancellationToken);

        await act.Should().ThrowAsync<Exception>();
    }
}
