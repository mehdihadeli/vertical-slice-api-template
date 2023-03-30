using System.Reflection;
using Catalogs.Products;
using Catalogs.Shared.Data;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shared.Web.Extensions;

namespace Catalogs.Shared;

public static class Configurations
{
	public const string CatalogModulePrefixUri = "api/v{version:apiVersion}/catalogs";
	public static WebApplicationBuilder AddCatalogServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddDbContext<CatalogsDbContext>(options => options.UseInMemoryDatabase("catalogs"));
		builder.Services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(CatalogsAssemblyInfo).Assembly));
		builder.Services.AddAutoMapper(x => { x.AddProfile<ProductMappers>(); });

		builder.Services.AddCustomValidators(typeof(CatalogsAssemblyInfo).Assembly);
		builder.Services.AddValidatorsFromAssembly(typeof(CatalogsAssemblyInfo).Assembly);

		builder.AddModulesServices();

		return builder;
	}

	public static Task<WebApplication> ConfigureCatalogServices(this WebApplication app)
	{
		return app.ConfigureModules();
	}

	public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapModulesEndpoints();
	}

	private static IServiceCollection AddCustomValidators(
		this IServiceCollection services,
		Assembly assembly,
		ServiceLifetime serviceLifetime = ServiceLifetime.Transient
	)
	{
		// https://docs.fluentvalidation.net/en/latest/di.html
		// I have some problem with registering IQuery validators with this
		// services.AddValidatorsFromAssembly(assembly);
		services.Scan(
			scan =>
				scan.FromAssemblies(assembly)
					.AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
					.UsingRegistrationStrategy(RegistrationStrategy.Skip)
					.AsImplementedInterfaces()
					.WithLifetime(serviceLifetime)
		);

		return services;
	}
}