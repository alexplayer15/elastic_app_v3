FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY ../Directory.Packages.props .

COPY elastic_app_v3.dashboard/elastic_app_v3.dashboard.csproj src/elastic_app_v3.dashboard/

RUN dotnet restore src/elastic_app_v3.dashboard/elastic_app_v3.dashboard.csproj

COPY elastic_app_v3.dashboard/ src/elastic_app_v3.dashboard/

WORKDIR /source/src/elastic_app_v3.dashboard/

RUN dotnet build "elastic_app_v3.dashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "elastic_app_v3.dashboard.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "elastic_app_v3.dashboard.dll"]