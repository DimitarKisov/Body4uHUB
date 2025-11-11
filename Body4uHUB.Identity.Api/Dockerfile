# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Body4uHUB.Identity.Api/Body4uHUB.Identity.Api.csproj", "Body4uHUB.Identity.Api/"]
COPY ["Body4uHUB.Identity.Application/Body4uHUB.Identity.Application.csproj", "Body4uHUB.Identity.Application/"]
COPY ["Body4uHUB.Identity.Domain/Body4uHUB.Identity.Domain.csproj", "Body4uHUB.Identity.Domain/"]
COPY ["Body4uHUB.Shared/Body4uHUB.Shared.Domain.csproj", "Body4uHUB.Shared/"]
COPY ["Body4uHUB.Identity.Infrastructure/Body4uHUB.Identity.Infrastructure.csproj", "Body4uHUB.Identity.Infrastructure/"]
RUN dotnet restore "./Body4uHUB.Identity.Api/Body4uHUB.Identity.Api.csproj"
COPY . .
WORKDIR "/src/Body4uHUB.Identity.Api"
RUN dotnet build "./Body4uHUB.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Body4uHUB.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Body4uHUB.Identity.Api.dll"]