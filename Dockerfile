FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Directory.Packages.props .

COPY src/elastic_app_v3.api/elastic_app_v3.api.csproj src/elastic_app_v3.api/
COPY src/elastic_app_v3.application/elastic_app_v3.application.csproj src/elastic_app_v3.application/
COPY src/elastic_app_v3.infrastructure/elastic_app_v3.infrastructure.csproj src/elastic_app_v3.infrastructure/
COPY src/elastic_app_v3.domain/elastic_app_v3.domain.csproj src/elastic_app_v3.domain/

RUN dotnet restore src/elastic_app_v3.api/elastic_app_v3.api.csproj

COPY src/elastic_app_v3.api/ src/elastic_app_v3.api/
COPY src/elastic_app_v3.application/ src/elastic_app_v3.application/
COPY src/elastic_app_v3.infrastructure/ src/elastic_app_v3.infrastructure/
COPY src/elastic_app_v3.domain/ src/elastic_app_v3.domain/

WORKDIR /source/src/elastic_app_v3.api/

RUN dotnet build "elastic_app_v3.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "elastic_app_v3.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#Fix issue where Alpine runs with Globalization invariant mode which is not compatible with Microsoft.Data.SqlClient
RUN apk add --no-cache icu-libs=76.1-r1 icu-data-full=76.1-r1

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "elastic_app_v3.api.dll"]
