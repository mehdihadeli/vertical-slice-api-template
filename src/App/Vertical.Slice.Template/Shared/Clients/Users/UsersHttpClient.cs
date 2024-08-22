using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Shared.Core.Paging;
using Shared.Web.Extensions;
using Vertical.Slice.Template.Shared.Clients.Users.Dtos;
using Vertical.Slice.Template.Users;
using Vertical.Slice.Template.Users.Models;

namespace Vertical.Slice.Template.Shared.Clients.Users;

public class UsersHttpClient(HttpClient httpClient, IOptions<UsersHttpClientOptions> userHttpClientOptions)
    : IUsersHttpClient
{
    private readonly UsersHttpClientOptions _userHttpClientOptions = userHttpClientOptions.Value;

    public async Task<PageList<User>> GetAllUsersAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default
    )
    {
        // https://stackoverflow.com/a/67877742/581476
        var qb = new QueryBuilder
        {
            { "limit", pageRequest.PageSize.ToString(CultureInfo.InvariantCulture) },
            { "skip", pageRequest.PageNumber.ToString(CultureInfo.InvariantCulture) },
        };

        // https://github.com/App-vNext/Polly#handing-return-values-and-policytresult
        var httpResponse = await httpClient.GetAsync(
            $"{_userHttpClientOptions.UsersEndpoint}?{qb.ToQueryString().Value}",
            cancellationToken
        );

        // https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
        // throw HttpResponseException instead of HttpRequestException (because we want detail response exception) with corresponding status code
        await httpResponse.EnsureSuccessStatusCodeWithDetailAsync();

        var usersListPage = await httpResponse.Content.ReadFromJsonAsync<UsersListPageClientDto>(
            cancellationToken: cancellationToken
        );

        if (usersListPage is null)
            throw new Exception("users page list cannot be null");

        var mod = usersListPage.Total % usersListPage.Limit;
        var totalPageCount = (usersListPage.Total / usersListPage.Limit) + (mod == 0 ? 0 : 1);

        var items = usersListPage.Users.ToUsers();

        var pageList = new PageList<User>(items.ToList(), usersListPage.Skip, usersListPage.Limit, usersListPage.Total);

        return pageList;
    }
}
