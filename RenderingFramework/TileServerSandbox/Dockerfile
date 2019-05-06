FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 58888
EXPOSE 44379

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["TileServerSandbox/TileServerSandbox.csproj", "TileServerSandbox/"]
RUN dotnet restore "TileServerSandbox/TileServerSandbox.csproj"
COPY . .
WORKDIR "/src/TileServerSandbox"
RUN dotnet build "TileServerSandbox.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TileServerSandbox.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TileServerSandbox.dll"]