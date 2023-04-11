using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Shared.Web.Contracts;

public interface IModuleConfiguration
{
    WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder);

    Task<WebApplication> ConfigureModule(WebApplication app);

    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}
