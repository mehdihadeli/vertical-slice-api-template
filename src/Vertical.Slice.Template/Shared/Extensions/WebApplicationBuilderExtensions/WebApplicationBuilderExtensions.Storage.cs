using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vertical.Slice.Template.Shared.Abstractions.Core.Domain.Events;
using Vertical.Slice.Template.Shared.Abstractions.Ef;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.Shared.Data.Migrations;
using Vertical.Slice.Template.Shared.EF;
using Vertical.Slice.Template.Shared.EF.Extensions;
using Vertical.Slice.Template.Shared.Workers;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static void AddStorage(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetValue<bool>($"{nameof(PostgresOptions)}:{nameof(PostgresOptions.UseInMemory)}"))
        {
            builder.Services.AddDbContext<CatalogsDbContext>(options => options.UseInMemoryDatabase("Catalogs"));

            builder.Services.AddScoped<IDbFacadeResolver>(provider => provider.GetService<CatalogsDbContext>()!);
            builder.Services.AddScoped<IDomainEventContext>(provider => provider.GetService<CatalogsDbContext>()!);
        }
        else
        {
            builder.Services.AddPostgresDbContext<CatalogsDbContext>();

            builder.Services.AddHostedService<MigrationWorker>();
            builder.Services.AddHostedService<SeedWorker>();

            // add migration and seeders dependencies
            builder.Services.AddScoped<IMigrationExecutor, CatalogsMigrationExecutor>();
            //services.AddScoped<IDataSeeder, Seeder>();
        }
    }
}
