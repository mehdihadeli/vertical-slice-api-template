using Vertical.Slice.Template.Shared.Resiliency.Options;

namespace Vertical.Slice.Template.ApiClient.RickAndMorty;

public class RikAndMortyApiClientOptions : HttpClientOptions
{
    public string CharacterEndpoint { get; set; } = "api/character";
    public override string BaseAddress { get; set; } = "https://rickandmortyapi.com";
}
