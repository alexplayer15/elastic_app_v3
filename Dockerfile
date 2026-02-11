FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY src/elastic_app_v3.csproj src/

RUN dotnet restore src/elastic_app_v3.csproj

COPY src/ src/

WORKDIR /source/src

RUN dotnet build "elastic_app_v3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "elastic_app_v3.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#Fix issue where Alpine runs with Globalization invariant mode which is not compatible with Microsoft.Data.SqlClient
RUN apk add --no-cache icu-libs=76.1-r1 icu-data-full=76.1-r1

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "elastic_app_v3.dll"]
