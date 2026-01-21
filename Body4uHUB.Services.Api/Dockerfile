# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create DataProtection directory with correct permissions BEFORE switching to non-root user
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys && \
    chown -R app:app /home/app/.aspnet && \
    chmod -R 755 /home/app/.aspnet

# Switch to non-root user AFTER setting permissions
USER $APP_UID


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Body4uHUB.Services.Api/Body4uHUB.Services.Api.csproj", "Body4uHUB.Services.Api/"]
COPY ["Body4uHUB.Services.Application/Body4uHUB.Services.Application.csproj", "Body4uHUB.Services.Application/"]
COPY ["Body4uHUB.Services.Domain/Body4uHUB.Services.Domain.csproj", "Body4uHUB.Services.Domain/"]
COPY ["Body4uHUB.Shared/Body4uHUB.Shared.Domain.csproj", "Body4uHUB.Shared/"]
COPY ["Body4uHUB.Shared.Application/Body4uHUB.Shared.Application.csproj", "Body4uHUB.Shared.Application/"]
COPY ["Body4uHUB.Services.Infrastructure/Body4uHUB.Services.Infrastructure.csproj", "Body4uHUB.Services.Infrastructure/"]
COPY ["Body4uHUB.Shared.Infrastructure/Body4uHUB.Shared.Infrastructure.csproj", "Body4uHUB.Shared.Infrastructure/"]
COPY ["Body4uHUB.Shared.Api/Body4uHUB.Shared.Api.csproj", "Body4uHUB.Shared.Api/"]
RUN dotnet restore "./Body4uHUB.Services.Api/Body4uHUB.Services.Api.csproj"
COPY . .
WORKDIR "/src/Body4uHUB.Services.Api"
RUN dotnet build "./Body4uHUB.Services.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Body4uHUB.Services.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Body4uHUB.Services.Api.dll"]