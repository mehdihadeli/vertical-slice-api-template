# Vertical Slice API Template
[![NuGet](https://img.shields.io/nuget/v/Vertical.Slice.Template?style=flat-square)](https://www.nuget.org/packages/Vertical.Slice.Template)
[![CI-CD](https://img.shields.io/github/actions/workflow/status/mehdihadeli/vertical-slice-api-template/ci-cd.yml?style=flat-square)](https://github.com/mehdihadeli/vertical-slice-api-template/actions/workflows/ci-cd.yml)
[![Commitizen friendly](https://img.shields.io/badge/commitizen-friendly-brightgreen.svg?&style=flat-square)](http://commitizen.github.io/cz-cli/)

> This is a An `asp.net core template` based on `Vertical Slice Architecture`, CQRS, Minimal APIs, API Versioning and Swagger. Create a new project based on this template by clicking the above **Use this template** button or by installing and running the associated NuGet package (see Getting Started for full details). 

## Getting Started & Prerequisites
1. This application uses `Https` for hosting apis, to setup a valid certificate on your machine, you can create a [Self-Signed Certificate](https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0#macos-or-linux), see more about enforce certificate [here](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl).
2. Install git - [https://git-scm.com/downloads](https://git-scm.com/downloads).
3. Install .NET Core 7.0 - [https://dotnet.microsoft.com/download/dotnet/7.0](https://dotnet.microsoft.com/download/dotnet/7.0).
4. Install Visual Studio, Rider or VSCode.
5. Run `dotnet new install Vertical.Slice.Template` to install the project templates.
6. Now with running `dotnet new --list`, we should see `Vertical.Slice.Template` in the template list.
7. Create a folder for your solution and cd into it (the template will use it as project name)
8. Run `dotnet new vsa` for short name or `dotnet new Vertical.Slice.Template -n <YourProjectName>` to create a new project template.
9. Open [<YourProjectName>.sln](./Vertical.Slice.Template.sln) solution, make sure that's compiling.
9. Navigate to `src/App/<YourProjectName>.Api` and run `dotnet run` to launch the back end (ASP.NET Core Web API)
10. Open web browser https://localhost:5158/swagger Swagger UI

For install package locally you can use this command in the root of your cloned responsitory:
``` bash
dotnet new install .
```
