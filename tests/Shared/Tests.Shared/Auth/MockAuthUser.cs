using System.Security.Claims;

namespace Vertical.Slice.Template.TestsShared.Auth;

public sealed class MockAuthUser(params Claim[] claims)
{
    public List<Claim> Claims { get; } = claims.ToList();
}
