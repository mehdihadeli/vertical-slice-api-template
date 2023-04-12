# Vertical Slice API Template
[![NuGet](https://img.shields.io/nuget/v/Vertical.Slice.Template?style=flat-square)](https://www.nuget.org/packages/Vertical.Slice.Template)
[![CI](https://img.shields.io/github/actions/workflow/status/mehdihadeli/vertical-slice-api-template/dotnet.yml?style=flat-square)](https://github.com/mehdihadeli/vertical-slice-api-template/actions/workflows/dotnet.yml) 

This is a An asp.net core template based on `Vertical Slice Architecture`, CQRS, Minimal APIs, API Versioning and Swagger. Create a new project based on this template by clicking the above **Use this template** button or by installing and running the associated NuGet package (see Getting Started for full details). 


## Getting Started
1. Install the latest [.NET Core SDK](https://dot.net).
2. Run `dotnet new install Vertical.Slice.Template` to install the project templates.
3. Now with running `dotnet new --list`, we should see `Vertical.Slice.Template` in the template list.
4. Create a folder for your solution and cd into it (the template will use it as project name)
5. Run `dotnet new vsa` for short name or `dotnet new Vertical.Slice.Template` to create a new project
6. Navigate to `src/App/Vertical.Slice.Template.Api` and run `dotnet run` to launch the back end (ASP.NET Core Web API)
7. Open web browser https://localhost:5158/swagger Swagger UI

## Libraries
* [ASP.NET Core 7](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
* [Entity Framework Core 7](https://docs.microsoft.com/en-us/ef/core/)
* [MediatR](https://github.com/jbogard/MediatR)
* [AutoMapper](https://github.com/AutoMapper/AutoMapper)
* [FluentValidation](https://fluentvalidation.net/)
* [XUnit](https://github.com/xunit/xunit), [FluentAssertions](https://fluentassertions.com/), [NSubstitute](https://github.com/nsubstitute/NSubstitute)
* [Serilog](https://serilog.net/)

## Folder Structure
``` cmd
src
│   Directory.Build.props
│   Directory.Build.targets
│   Directory.Packages.props
│
├───ApiClient
│   │   ApiClient.csproj
│   │   ApiClientOptions.cs
│   │   nswag.json
│   │   swagger.json
│   │
│   ├───Catalogs
│   │   │   CatalogsMappingProfile.cs
│   │   │   CatalogsService.cs
│   │   │   ICatalogsService.cs
│   │   │
│   │   └───Dtos
│   │           CreateProductInput.cs
│   │           CreateProductOutput.cs
│   │           GetProductByIdOutput.cs
│   │           GetProductByPageOutput.cs
│   │           GetProductsByPageInput.cs
│   │           ProductDto.cs
│   │           ProductLiteDto.cs
│   │
│   └───Extensions
│           ServiceCollectionExtensions.cs
│
├───App
│   ├───Vertical.Slice.Template
│   │   │   CatalogsAssemblyInfo.cs
│   │   │   Vertical.Slice.Template.csproj
│   │   │
│   │   ├───Products
│   │   │   │   ProductConfigurations.cs
│   │   │   │   ProductMappingProfiles.cs
│   │   │   │
│   │   │   ├───Data
│   │   │   │       ProductEntityTypeConfigurations.cs
│   │   │   │       SieveProductReadConfigurations.cs
│   │   │   │
│   │   │   ├───Dtos
│   │   │   │       ProductDto.cs
│   │   │   │
│   │   │   ├───Features
│   │   │   │   ├───CreatingProduct
│   │   │   │   │   └───v1
│   │   │   │   │           CreateProduct.cs
│   │   │   │   │           CreateProductEndpoint.cs
│   │   │   │   │           ProductCreated.cs
│   │   │   │   │
│   │   │   │   ├───GettingProductById
│   │   │   │   │   └───v1
│   │   │   │   │           GetProductById.cs
│   │   │   │   │           GetProductByIdEndpoint.cs
│   │   │   │   │
│   │   │   │   └───GettingProductsByPage
│   │   │   │       └───v1
│   │   │   │               GetProductsByPage.cs
│   │   │   │               GetProductsByPageEndpoint.cs
│   │   │   │
│   │   │   ├───Models
│   │   │   │       Product.cs
│   │   │   │
│   │   │   └───ReadModel
│   │   │           ProductReadModel.cs
│   │   │
│   │   └───Shared
│   │       ├───Data
│   │       │       CatalogsDbContext.cs
│   │       │
│   │       └───Extensions
│   │               WebApplicationBuilderExtensions.Infrastrcture.cs
│   │
│   └───Vertical.Slice.Template.Api
│       │   appsettings.Development.json
│       │   appsettings.json
│       │   Program.cs
│       │   Vertical.Slice.Template.Api.csproj
│       │
│       ├───Extensions
│       │   └───WebApplicationBuilderExtensions
│       │           WebApplicationBuilderExtensions.ProblemDetails.cs
│       │           WebApplicationBuilderExtensions.Versioning.cs
│       │
│       └───Properties
│               launchSettings.json
│
└───Shared
    │   Shared.csproj
    │
    ├───Core
    │   │   ApplicationSieveProcessor.cs
    │   │   IDbExecutors.cs
    │   │
    │   ├───Contracts
    │   │       IDomainEvent.cs
    │   │       IEvent.cs
    │   │       IPageList.cs
    │   │       IPageQuery.cs
    │   │       IPageRequest.cs
    │   │
    │   ├───Domain
    │   │       DomainEvent.cs
    │   │
    │   ├───Exceptions
    │   │       AppException.cs
    │   │       BadRequestException.cs
    │   │       ConflictException.cs
    │   │       CustomException.cs
    │   │       HttpResponseException.cs
    │   │       NotFoundException.cs
    │   │       ValidationException.cs
    │   │
    │   ├───Extensions
    │   │   │   QueryableExtensions.cs
    │   │   │   ValidationExtensions.cs
    │   │   │
    │   │   └───ServiceCollectionsExtensions
    │   │           ServiceCollectionExtensions.DbExecutorExtensions.cs
    │   │
    │   ├───Id
    │   │       IdGenerator.cs
    │   │
    │   ├───Reflection
    │   │   │   ReflectionUtilities.cs
    │   │   │
    │   │   └───Extensions
    │   │           AssemblyExtensions.cs
    │   │           TypeExtensions.cs
    │   │
    │   └───Types
    │           Event.cs
    │           PageList.cs
    │           PageQuery.cs
    │           PageRequest.cs
    │
    ├───EF
    │   └───Extensions
    │           DbContextExtensions.cs
    │
    ├───Logging
    │       Extensions.cs
    │       SerilogOptions.cs
    │
    ├───Swagger
    │       ConfigureSwaggerOptions.cs
    │       SwaggerDefaultValues.cs
    │
    ├───Validation
    │   │   RequestValidationBehavior.cs
    │   │   ValidationError.cs
    │   │   ValidationResultModel.cs
    │   │
    │   └───Extensions
    │           RegistrationExtensions.cs
    │           ValidatorExtension.cs
    │
    └───Web
        │   PolicyOptions.cs
        │
        ├───Contracts
        │       IHttpCommand.cs
        │       IHttpQuery.cs
        │       IMinimalEndpoint.cs
        │       IModuleConfiguration.cs
        │       IProblemDetailMapper.cs
        │       ISharedModulesConfiguration.cs
        │
        ├───Extensions
        │   │   ConfigurationExtensions.cs
        │   │   HeaderDictionaryExtensions.cs
        │   │   HttpResponseMessageExtensions.cs
        │   │   QueryCollectionExtensions.cs
        │   │
        │   └───ServiceCollection
        │           ServiceCollectionExtensions.Dependency.cs
        │           ServiceCollectionExtensions.Options.cs
        │
        ├───Middlewares
        │   └───CaptureExceptionMiddleware
        │           CaptureExceptionMiddlewareExtensions.cs
        │           CaptureExceptionMiddlewareImp.cs
        │
        ├───Minimal
        │   │   HttpCommand.cs
        │   │   HttpQuery.cs
        │   │
        │   └───Extensions
        │           EndpointConventionBuilderExtensions.cs
        │           EndpointRouteBuilderExtensions.cs
        │           MinimalApiExtensions.cs
        │           ModuleExtensions.cs
        │           TypedResultsExtensions.cs
        │
        └───ProblemDetail
            │   DefaultProblemDetailMapper.cs
            │   ProblemDetailsService.cs
            │   ProblemDetailsWriter.cs
            │   RegistrationExtensions.cs
            │   ResponseMetadata.cs
            │
            └───HttpResults
                    InternalHttpProblemResult.cs
                    NotFoundHttpProblemResult.cs
                    UnAuthorizedHttpProblemResult.cs
```