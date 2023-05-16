using System.Net.Http.Json;
using AutoMapper;
using Microsoft.Extensions.Options;
using Vertical.Slice.Template.ApiClient.RickAndMorty.Dtos;
using Vertical.Slice.Template.ApiClient.RickAndMorty.Model;
using Vertical.Slice.Template.Shared.Core.Extensions;
using Vertical.Slice.Template.Shared.Web.Extensions;

namespace Vertical.Slice.Template.ApiClient.RickAndMorty;

public class RickAndMortyClient : IRickAndMortyClient
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly RikAndMortyApiClientOptions _options;

    public RickAndMortyClient(HttpClient httpClient, IMapper mapper, IOptions<RikAndMortyApiClientOptions> options)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _options = options.Value;
    }

    public async Task<Character> GetCharacterById(long id, CancellationToken cancellationToken = default)
    {
        id.NotBeNegativeOrZero();

        // https://github.com/App-vNext/Polly#handing-return-values-and-policytresult
        var httpResponse = await _httpClient.GetAsync($"{_options.CharacterEndpoint}/{id}", cancellationToken);

        // https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
        // throw HttpResponseException instead of HttpRequestException (because we want detail response exception) with corresponding status code
        await httpResponse.EnsureSuccessStatusCodeWithDetailAsync();

        var characterResponseClientDto = await httpResponse.Content.ReadFromJsonAsync<CharacterResponseClientDto>(
            cancellationToken: cancellationToken
        );

        var photovoltaicSystem = _mapper.Map<Character>(characterResponseClientDto);

        return photovoltaicSystem;
    }
}
