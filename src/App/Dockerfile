# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG TARGETARCH
ARG CONFIGURATION=Release
WORKDIR /src

COPY ./.editorconfig ./
COPY ./nuget.config ./
COPY ./global.json ./
COPY ./Directory.Build.props /Directory.Build.props
COPY ./src/Directory.Build.props ./
COPY ./src/Directory.Build.targets ./
COPY ./src/Directory.Packages.props ./

COPY ./src/Shared/Shared.csproj ./Shared/
COPY ./src/App/Vertical.Slice.Template/Vertical.Slice.Template.csproj ./Application/Vertical.Slice.Template/
COPY ./src/App/Vertical.Slice.Template.Api/Vertical.Slice.Template.Api.csproj ./Application/Vertical.Slice.Template.Api/
COPY ./src/Aspire/Vertical.Slice.Template.ServiceDefaults/Vertical.Slice.Template.ServiceDefaults.csproj ./Aspire/Vertical.Slice.Template.ServiceDefaults/
COPY ./src/ApiClients/Vertical.Slice.Template.KiotaClients/Vertical.Slice.Template.KiotaClients.csproj ./ApiClients/Vertical.Slice.Template.KiotaClients/
COPY ./src/ApiClients/Vertical.Slice.Template.ConnectedServiceClients/Vertical.Slice.Template.ConnectedServiceClients.csproj ./ApiClients/Vertical.Slice.Template.ConnectedServiceClients/

RUN --mount=type=cache,id=vertical-slice-nuget,target=/root/.nuget/packages,sharing=locked \
    dotnet restore ./Application/Vertical.Slice.Template.Api/Vertical.Slice.Template.Api.csproj -p:SkipVersion=true -a $TARGETARCH

COPY ./src/Shared/ ./Shared/
COPY ./src/App/Vertical.Slice.Template/ ./Application/Vertical.Slice.Template/
COPY ./src/App/Vertical.Slice.Template.Api/ ./Application/Vertical.Slice.Template.Api/
COPY ./src/Aspire/Vertical.Slice.Template.ServiceDefaults/ ./Aspire/Vertical.Slice.Template.ServiceDefaults/
COPY ./src/ApiClients/Vertical.Slice.Template.KiotaClients/ ./ApiClients/Vertical.Slice.Template.KiotaClients/
COPY ./src/ApiClients/Vertical.Slice.Template.ConnectedServiceClients/ ./ApiClients/Vertical.Slice.Template.ConnectedServiceClients/

WORKDIR /src/Application/Vertical.Slice.Template.Api
RUN --mount=type=cache,id=vertical-slice-nuget,target=/root/.nuget/packages,sharing=locked \
    dotnet build -c $CONFIGURATION --no-restore

FROM build AS publish
ARG TARGETARCH
ARG CONFIGURATION=Release
RUN --mount=type=cache,id=vertical-slice-nuget,target=/root/.nuget/packages,sharing=locked \
    dotnet publish -c $CONFIGURATION --no-build --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vertical.Slice.Template.Api.dll"]
