using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vertical.Slice.Template.Shared.Abstractions.Core.Domain.Events;
using Vertical.Slice.Template.Shared.Abstractions.Ef;
using Vertical.Slice.Template.Shared.Core.Extensions;
using Vertical.Slice.Template.Shared.Core.Extensions.ServiceCollectionsExtensions;
using Vertical.Slice.Template.Shared.EF.Interceptors;

namespace Vertical.Slice.Template.Shared.EF.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresDbContext<TDbContext>(
        this IServiceCollection services,
        Assembly? migrationAssembly = null,
        Action<DbContextOptionsBuilder>? builder = null,
        params Assembly[] assembliesToScan
    )
        where TDbContext : DbContext, IDbFacadeResolver, IDomainEventContext
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddValidatedOptions<PostgresOptions>(nameof(PostgresOptions));

        services.AddScoped<IConnectionFactory>(sp =>
        {
            var postgresOptions = sp.GetRequiredService<PostgresOptions>();
            postgresOptions.ConnectionString.NotBeNullOrWhiteSpace();
            return new NpgsqlConnectionFactory(postgresOptions.ConnectionString);
        });

        services.AddDbContext<TDbContext>(
            (sp, options) =>
            {
                var postgresOptions = sp.GetRequiredService<PostgresOptions>();

                options
                    .UseNpgsql(
                        postgresOptions.ConnectionString,
                        sqlOptions =>
                        {
                            var name =
                                migrationAssembly?.GetName().Name
                                ?? postgresOptions.MigrationAssembly
                                ?? typeof(TDbContext).Assembly.GetName().Name;

                            sqlOptions.MigrationsAssembly(name);
                            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        }
                    )
                    // https://github.com/efcore/EFCore.NamingConventions
                    .UseSnakeCaseNamingConvention();

                // ref: https://andrewlock.net/series/using-strongly-typed-entity-ids-to-avoid-primitive-obsession/
                options.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<long>>();

                options.AddInterceptors(
                    new AuditInterceptor(),
                    new SoftDeleteInterceptor(),
                    new ConcurrencyInterceptor()
                );

                builder?.Invoke(options);
            }
        );

        services.AddScoped<IDbFacadeResolver>(provider => provider.GetService<TDbContext>()!);
        services.AddScoped<IDomainEventContext>(provider => provider.GetService<TDbContext>()!);

        return services;
    }
}
