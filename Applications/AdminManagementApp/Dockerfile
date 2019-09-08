#FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["AdminManagementApp.csproj", "AdminManagementApp/"]
COPY NuGet.config /src/NuGet/
RUN dotnet restore "AdminManagementApp/AdminManagementApp.csproj"
COPY . .
WORKDIR "/src/AdminManagementApp"
RUN dotnet build "AdminManagementApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AdminManagementApp/AdminManagementApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AdminManagementApp.dll"]