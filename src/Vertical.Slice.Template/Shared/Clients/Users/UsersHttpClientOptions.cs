using Vertical.Slice.Template.Shared.Resiliency.Options;

namespace Vertical.Slice.Template.Shared.Clients.Users;

public class UsersHttpClientOptions : HttpClientOptions
{
    public override string BaseAddress { get; set; } = default!;
    public string UsersEndpoint { get; set; } = default!;
}
