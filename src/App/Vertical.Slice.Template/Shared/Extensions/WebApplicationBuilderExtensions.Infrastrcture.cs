using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Core.Extensions;
using Shared.Logging;
using Shared.Swagger;
using Shared.Validation;
using Shared.Validation.Extensions;
using Sieve.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Shared.Data;

namespace Vertical.Slice.Template.Shared.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructures(this WebApplicationBuilder builder)
    {
        builder.AddCustomSerilog();

        builder.Services.AddDbContext<CatalogsDbContext>(options => options.UseInMemoryDatabase("catalogs"));
        builder.Services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(CatalogsAssemblyInfo).Assembly));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddAutoMapper(x =>
        {
            x.AddProfile<ProductMappingProfiles>();
        });
        builder.Services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
        builder.Services.AddCustomValidators(typeof(CatalogsAssemblyInfo).Assembly);
        builder.Services.ScanAndRegisterDbExecutors();

        return builder;
    }
}
