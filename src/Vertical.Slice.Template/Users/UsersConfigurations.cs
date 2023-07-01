using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Vertical.Slice.Template.Shared;
using Vertical.Slice.Template.Users.GetUsers;

namespace Vertical.Slice.Template.Users;

public static class UsersConfigurations
{
    public const string Tag = "Users";
    public const string UsersPrefixUri = $"{CatalogsConfigurations.CatalogsPrefixUri}/users";

    public static WebApplicationBuilder AddUsersModuleServices(this WebApplicationBuilder builder)
    {
        return builder;
    }

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var todoItems = routeBuilder
            .NewVersionedApi("Users")
            .AddEndpointFilter(
                async (efiContext, next) =>
                {
                    var stopwatch = Stopwatch.StartNew();
                    var result = await next(efiContext);
                    stopwatch.Stop();
                    var elapsed = stopwatch.ElapsedMilliseconds;
                    var response = efiContext.HttpContext.Response;
                    response.Headers.Add("X-Response-Time", $"{elapsed} milliseconds");
                    return result;
                }
            );

        var usersV1 = todoItems.MapGroup(UsersPrefixUri).HasApiVersion(1.0);

        usersV1.MapGetUsersByPageEndpoint();

        return routeBuilder;
    }
}
