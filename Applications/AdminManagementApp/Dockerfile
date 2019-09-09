#FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
ARG FEED_URL
ARG PAT

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["AdminManagementApp.csproj", "AdminManagementApp/"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AdminManagementApp.dll"]