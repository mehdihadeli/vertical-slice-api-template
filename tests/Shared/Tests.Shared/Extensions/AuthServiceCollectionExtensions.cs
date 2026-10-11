using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Vertical.Slice.Template.TestsShared.Auth;

namespace Vertical.Slice.Template.TestsShared.Extensions;

public static class AuthServiceCollectionExtensions
{
    public static AuthenticationBuilder AddTestAuthentication(
        this IServiceCollection services,
        params System.Security.Claims.Claim[] claims
    )
    {
        services.AddSingleton(new MockAuthUser(claims));
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(Constants.Auth.Scheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        return services
            .AddAuthentication(Constants.Auth.Scheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                Constants.Auth.Scheme,
                _ => { }
            );
    }
}
