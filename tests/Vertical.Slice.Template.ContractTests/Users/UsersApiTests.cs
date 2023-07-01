using System.Net.Http.Json;
using FluentAssertions;
using Vertical.Slice.Template.Shared.Clients.Users.Dtos;

namespace Vertical.Slice.Template.ContractTests.Users;

public class UsersApiTests
{
    private readonly HttpClient _httpClient;

    public UsersApiTests()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://dummyjson.com"), };
    }

    [Fact]
    public async Task contracts_should_pass_for_get_character_by_id()
    {
        // Arrange
        var pageSize = 10;
        var pageNumber = 1;

        // Act
        var response = await _httpClient.GetAsync($"/users?limit={pageSize}&skip={pageNumber}");
        response.EnsureSuccessStatusCode();
        var usersListPage = await response.Content.ReadFromJsonAsync<UsersListPageClientDto>();

        // Assert
        usersListPage.Should().NotBeNull();
        usersListPage!.Limit.Should().Be(pageSize);
        usersListPage!.Skip.Should().Be(pageNumber);
        usersListPage.Users.Should().NotBeNull();
        usersListPage.Users.Should().HaveCountGreaterThan(0);
        var user = usersListPage.Users.First();
        user.Address.Should().NotBeNull();
        user.Id.Should().BeGreaterThan(0);
        user.FirstName.Should().NotBeNullOrEmpty();
        user.LastName.Should().NotBeNullOrEmpty();
        user.Email.Should().NotBeNullOrEmpty();
    }
}
