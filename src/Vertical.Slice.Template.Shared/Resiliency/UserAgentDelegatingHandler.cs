using System.Net.Http.Headers;
using System.Reflection;

namespace Vertical.Slice.Template.Shared.Resiliency;

public class UserAgentDelegatingHandler : DelegatingHandler
{
    public UserAgentDelegatingHandler()
        : this(Assembly.GetEntryAssembly()) { }

    public UserAgentDelegatingHandler(Assembly? assembly)
        : this(GetProduct(assembly), GetVersion(assembly)) { }

    public UserAgentDelegatingHandler(string? applicationName, string? applicationVersion)
    {
        if (applicationName == null)
        {
            throw new ArgumentNullException(nameof(applicationName));
        }

        if (applicationVersion == null)
        {
            throw new ArgumentNullException(nameof(applicationVersion));
        }

        UserAgentValues = new List<ProductInfoHeaderValue>()
        {
            new(applicationName.Replace(' ', '-'), applicationVersion),
            new($"({Environment.OSVersion})"),
        };
    }

    public UserAgentDelegatingHandler(List<ProductInfoHeaderValue> userAgentValues) =>
        UserAgentValues = userAgentValues ?? throw new ArgumentNullException(nameof(userAgentValues));

    public IList<ProductInfoHeaderValue> UserAgentValues { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        if (!request.Headers.UserAgent.Any())
        {
            foreach (var userAgentValue in UserAgentValues)
            {
                request.Headers.UserAgent.Add(userAgentValue);
            }
        }

        // Else the header has already been added due to a retry.
        return base.SendAsync(request, cancellationToken);
    }

    private static string? GetProduct(Assembly? assembly) =>
        assembly?.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

    private static string? GetVersion(Assembly? assembly) =>
        assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
}
