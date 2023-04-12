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
