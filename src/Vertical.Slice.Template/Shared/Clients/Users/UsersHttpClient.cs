using System.Globalization;
using System.Net.Http.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Vertical.Slice.Template.Shared.Clients.Users.Dtos;
using Vertical.Slice.Template.Shared.Core.Paging;
using Vertical.Slice.Template.Shared.Web.Extensions;
using Vertical.Slice.Template.Users.Models;

namespace Vertical.Slice.Template.Shared.Clients.Users;

public class UsersHttpClient : IUsersHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly UsersHttpClientOptions _userHttpClientOptions;

    public UsersHttpClient(
        HttpClient httpClient,
        IMapper mapper,
        IOptions<UsersHttpClientOptions> userHttpClientOptions
    )
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _userHttpClientOptions = userHttpClientOptions.Value;
    }

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
        var httpResponse = await _httpClient.GetAsync(
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

        var items = _mapper.Map<IEnumerable<User>>(usersListPage.Users);

        var pageList = new PageList<User>(items.ToList(), usersListPage.Skip, usersListPage.Limit, usersListPage.Total);

        return pageList;
    }
}
