# Build from the repository root, not from this directory, so that the
# ProjectReference paths in the .csproj files resolve:
#
#   docker build -f src/Services/Identity/NexusSupport.Identity.Api/Dockerfile -t identity-api .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Restore against the project files alone. This layer is only invalidated when a
# .csproj changes, so editing source does not re-download every package.
COPY src/Services/Identity/NexusSupport.Identity.Api/NexusSupport.Identity.Api.csproj \
     src/Services/Identity/NexusSupport.Identity.Api/
COPY src/Services/Identity/NexusSupport.Identity.Application/NexusSupport.Identity.Application.csproj \
     src/Services/Identity/NexusSupport.Identity.Application/
COPY src/Services/Identity/NexusSupport.Identity.Infrastructure/NexusSupport.Identity.Infrastructure.csproj \
     src/Services/Identity/NexusSupport.Identity.Infrastructure/
COPY src/Services/Identity/NexusSupport.Identity.Domain/NexusSupport.Identity.Domain.csproj \
     src/Services/Identity/NexusSupport.Identity.Domain/
RUN dotnet restore src/Services/Identity/NexusSupport.Identity.Api/NexusSupport.Identity.Api.csproj

# Only the Identity service is copied: the API references Application,
# Infrastructure and Domain, and nothing else in the solution.
COPY src/Services/Identity/ src/Services/Identity/

RUN dotnet publish src/Services/Identity/NexusSupport.Identity.Api/NexusSupport.Identity.Api.csproj \
    --configuration $BUILD_CONFIGURATION \
    --no-restore \
    --output /app/publish \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Must match identityTargetPort in infra/identity/main.bicep. Kestrel listens on
# 8080 by default in these images; setting it explicitly keeps the contract with
# the Container App ingress visible here rather than implied.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

# APP_UID is defined by the base image; run as that non-root user.
USER $APP_UID

ENTRYPOINT ["dotnet", "NexusSupport.Identity.Api.dll"]
