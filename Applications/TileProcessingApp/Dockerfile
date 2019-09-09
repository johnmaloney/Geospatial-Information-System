FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["TileProcessingApp/TileProcessingApp.csproj", "TileProcessingApp/"]
RUN dotnet restore "TileProcessingApp/TileProcessingApp.csproj"
COPY . .
WORKDIR "/src/TileProcessingApp"
RUN dotnet build "TileProcessingApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TileProcessingApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TileProcessingApp.dll"]