using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using Polly.Wrap;
using Shared.Core.Paging;
using Shared.Resiliency.Options;
using Shared.Web.Extensions;
using Vertical.Slice.Template.Shared.Clients.Users.Dtos;
using Vertical.Slice.Template.Users;
using Vertical.Slice.Template.Users.Models;

namespace Vertical.Slice.Template.Shared.Clients.Users;

public class UsersHttpClient : IUsersHttpClient
{
    private readonly HttpClient _client;
    private readonly UsersHttpClientOptions _userHttpClientOptions;
    private readonly AsyncPolicyWrap<HttpResponseMessage> _combinedPolicy;

    public UsersHttpClient(
        HttpClient client,
        IOptions<UsersHttpClientOptions> userHttpClientOptions,
        IOptions<PolicyOptions> policyOptions
    )
    {
        _client = client;
        _userHttpClientOptions = userHttpClientOptions.Value;
        var policyOptionsValue = policyOptions.Value;

        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .RetryAsync(policyOptionsValue.RetryPolicyOptions.Count);

        // HttpClient itself will still enforce its own timeout, which is 100 seconds by default. To fix this issue, you need to set the HttpClient.Timeout property to match or exceed the timeout configured in Polly's policy.
        var timeoutPolicy = Policy.TimeoutAsync(
            policyOptionsValue.TimeoutPolicyOptions.TimeoutInSeconds,
            TimeoutStrategy.Pessimistic
        );

        // at any given time there will 3 parallel requests execution for specific service call and another 6 requests for other services can be in the queue. So that if the response from customer service is delayed or blocked then we don’t use too many resources
        var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(3, 6);

        // https://github.com/App-vNext/Polly#handing-return-values-and-policytresult
        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                policyOptionsValue.RetryPolicyOptions.Count + 1,
                TimeSpan.FromSeconds(policyOptionsValue.CircuitBreakerPolicyOptions.DurationOfBreak)
            );

        var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, bulkheadPolicy);

        _combinedPolicy = combinedPolicy.WrapAsync(timeoutPolicy);
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
        var httpResponse = await _combinedPolicy.ExecuteAsync(async () =>
        {
            // https://ollama.com/blog/openai-compatibility
            // https://www.youtube.com/watch?v=38jlvmBdBrU
            // https://platform.openai.com/docs/api-reference/chat/create
            // https://github.com/ollama/ollama/blob/main/docs/api.md#generate-a-chat-completion
            var response = await _client.GetAsync(
                $"{
                                                          _userHttpClientOptions.UsersEndpoint
                                                      }?{
                                                          qb.ToQueryString().Value
                                                      }",
                cancellationToken
            );

            return response;
        });

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
