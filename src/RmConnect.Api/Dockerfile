# Build from the repo root: docker build -f src/RmConnect.Api/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /repo

# Restore first so the package layer is cached between code changes
COPY Directory.Build.props nuget.config ./
COPY src/RmConnect.Domain/RmConnect.Domain.csproj src/RmConnect.Domain/
COPY src/RmConnect.Application/RmConnect.Application.csproj src/RmConnect.Application/
COPY src/RmConnect.Infrastructure/RmConnect.Infrastructure.csproj src/RmConnect.Infrastructure/
COPY src/RmConnect.Api/RmConnect.Api.csproj src/RmConnect.Api/
RUN dotnet restore src/RmConnect.Api/RmConnect.Api.csproj

COPY src/ src/
RUN dotnet publish src/RmConnect.Api/RmConnect.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "RmConnect.Api.dll"]
