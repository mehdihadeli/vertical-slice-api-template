using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Abstractions.Ef;
using Shared.EF;
using Shared.EF.Extensions;
using Vertical.Slice.Template.Shared.Data;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static void AddStorage(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetValue<bool>($"{nameof(PostgresOptions)}:{nameof(PostgresOptions.UseInMemory)}"))
        {
            builder.Services.AddDbContext<CatalogsDbContext>(options => options.UseInMemoryDatabase("Catalogs"));

            builder.Services.AddScoped<IDbFacadeResolver>(provider => provider.GetService<CatalogsDbContext>()!);
        }
        else
        {
            builder.Services.AddPostgresDbContext<CatalogsDbContext>();
        }
    }
}
