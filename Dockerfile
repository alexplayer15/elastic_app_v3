FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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

ENTRYPOINT ["dotnet", "elastic_app_v3.dll"]
