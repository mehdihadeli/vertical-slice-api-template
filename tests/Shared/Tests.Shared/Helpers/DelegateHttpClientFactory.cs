namespace Vertical.Slice.Template.TestsShared.Helpers;

public sealed class DelegateHttpClientFactory(Func<string, Lazy<HttpClient>> httpClientProvider) : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return httpClientProvider(name).Value;
    }
}
