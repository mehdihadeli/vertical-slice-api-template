FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app
#https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/
#https://swimburger.net/blog/dotnet/how-to-get-aspdotnet-core-server-urls
#https://tymisko.hashnode.dev/developing-aspnet-core-apps-in-docker-live-recompilat
#https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS http://*:80;https://*:443
ENV ASPNETCORE_ENVIRONMENT Production

# # https://code.visualstudio.com/docs/containers/troubleshooting#_running-as-a-nonroot-user
# # https://baeldung.com/ops/root-user-password-docker-container
# # https://stackoverflow.com/questions/52070171/whats-the-default-user-for-docker-exec
# # https://medium.com/redbubble/running-a-docker-container-as-a-non-root-user-7d2e00f8ee15
# # Creates a non-root user with an explicit UID and adds permission to access the /app folder
# # if we don't define a user container will use root user
# RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
# USER appuser

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src

# path are related to build context, here for us build context is root folder
# https://docs.docker.com/build/building/context/
COPY ./.editorconfig ./
COPY ./nuget.config ./
COPY ./global.json ./

COPY ./src/Directory.Build.props ./
COPY ./src/Directory.Build.targets ./
COPY ./src/Directory.Packages.props ./

# https://docs.docker.com/build/cache/#order-your-layers
# with any changes in csproj files all downstream layer will rebuil, so dotnet restore will execute again

## Building Blocks
COPY ./src/Shared/Shared.csproj ./Shared/

# Application
COPY ./src/App/Vertical.Slice.Template/Vertical.Slice.Template.csproj ./Application/Vertical.Slice.Template/
COPY ./src/App/Vertical.Slice.Template.Api/Vertical.Slice.Template.Api.csproj ./Application/Vertical.Slice.Template.Api/

# ApiClients
COPY ./src/ApiClients/Vertical.Slice.Template.KiotaClients/Vertical.Slice.Template.KiotaClients.csproj ./ApiClients/Vertical.Slice.Template.KiotaClients/
COPY ./src/ApiClients/Vertical.Slice.Template.ConnectedServiceClients/Vertical.Slice.Template.ConnectedServiceClients.csproj ./ApiClients/Vertical.Slice.Template.ConnectedServiceClients/

# https://docs.docker.com/build/cache/
# https://docs.docker.com/build/cache/#order-your-layers
# https://github.com/moby/buildkit/blob/master/frontend/dockerfile/docs/reference.md#run---mounttypecache
# https://github.com/dotnet/dotnet-docker/issues/3353
# https://stackoverflow.com/questions/69464184/using-docker-buildkit-mount-type-cache-for-caching-nuget-packages-for-net-5-d
# https://pythonspeed.com/articles/docker-cache-pip-downloads/
# When we have a chnage in a layer that layer and all subsequent layer will rebuild again
RUN dotnet restore ./Application/Vertical.Slice.Template.Api/Vertical.Slice.Template.Api.csproj

# Copy project files
COPY ./src/Shared/ ./Shared/
COPY ./src/App/Vertical.Slice.Template/  ./Application/Vertical.Slice.Template/
COPY ./src/App/Vertical.Slice.Template.Api/  ./Application/Vertical.Slice.Template.Api/
COPY ./src/ApiClients/Vertical.Slice.Template.KiotaClients/  ./ApiClients/Vertical.Slice.Template.KiotaClients/
COPY ./src/ApiClients/Vertical.Slice.Template.ConnectedServiceClients/  ./ApiClients/Vertical.Slice.Template.ConnectedServiceClients/

WORKDIR /src/Application/Vertical.Slice.Template.Api/

RUN dotnet build -c $configuration --no-restore 

FROM build AS publish
ARG configuration=Release

# Publish project to output folder and no build and restore, as we did it already
# https://stackoverflow.com/questions/5457095/release-generating-pdb-files-why
# pdbs also generate for release mode (pdbonly) so vsdb can use it for debugging for debug mode its default is (full)
RUN dotnet publish -c $configuration --no-build --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
# Setup working directory for the project
WORKDIR /app
COPY --from=publish /app/publish .

# for debug mode we change entrypoint with '--entrypoint' in 'docker run' for prevent runing application in this stage because we want to run container app with debugger launcher
#https://docs.docker.com/engine/reference/run/#entrypoint-default-command-to-execute-at-runtime
#https://oprea.rocks/blog/how-to-properly-override-the-entrypoint-using-docker-run

# https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/
# when we `run` app `dll`, inner `api project` working directory (will resolve to current working directory for app) that contains appsetings.json files or inner `bin directory` because when run app dll in this directory `app working directory` and `current working directory` will be set bin and because appsettings.json are there, so app can find this `appsettings.json` files in current working directory but if we run app dll outside this directories app current working directory will be changed, and it can't find `appsettings.json` files in current working directory, so we should explicitly specify working dir in to `bin` or `app project` folder, this problem doesn't exist for `.csproj files` and their working dir always resolve `correctly`
# in this layer we don't have nugets so we can use mounted volume in `docker run` or `docker-compose up` for this entrypoint when docker container will be run for the `host` with --mount type=bind,source=${env:USERPROFILE}\\.nuget\\packages,destination=/root/.nuget/packages,readonly, for example dotnet <application.dll> --additionalProbingPath /root/nuget/packages --additionalProbingPath ~/.nuget/packages
ENTRYPOINT ["dotnet", "Vertical.Slice.Template.Api.dll"]
