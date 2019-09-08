#FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
ARG FEED_URL
ARG PAT

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["AdminManagementApp.csproj", "AdminManagementApp/"]

# Optional: Sometimes the http client hangs because of a .NEt issue. Setting this in dockerfile helps 
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
# Environment variable to enable seesion token cache. More on this here: https://github.com/Microsoft/artifacts-credprovider#help
ENV NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED true

# Environment variable for adding endpoint credentials. More on this here: https://github.com/Microsoft/artifacts-credprovider#help
# Add "FEED_URL" AND "PAT" using --build-arg in docker build step. "endpointCredentials" field is an array, you can add multiple endpoint configurations.
# Make sure that you *do not* hard code the "PAT" here. That is a sensitive information and must not be checked in.
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS {\"endpointCredentials\": [{\"endpoint\":\"https://aetos.pkgs.visualstudio.com/_packaging/GIS-Packages/nuget/v3/index.json\", \"username\":\"maloney1@hotmail.com\", \"password\":\"o4wrt3525ow3xgpcshie7zo6stkqs7l7mxnuiqfl6xuuukslfqkq\"}]}

#COPY NuGet.config /src/NuGet/
RUN dotnet restore "AdminManagementApp/AdminManagementApp.csproj" --source "https://aetos.pkgs.visualstudio.com/_packaging/GIS-Packages/nuget/v3/index.json"
COPY . .
WORKDIR "/src/AdminManagementApp"
RUN dotnet build "AdminManagementApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AdminManagementApp/AdminManagementApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AdminManagementApp.dll"]