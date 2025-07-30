# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConfigMapster/ConfigMapster.csproj", "ConfigMapster/"]
COPY ["ConfigMapster.API.ApplicationService/ConfigMapster.API.ApplicationService.csproj", "ConfigMapster.API.ApplicationService/"]
COPY ["ConfigMapster.API.Domain/ConfigMapster.API.Domain.csproj", "ConfigMapster.API.Domain/"]
COPY ["ConfigMapster.API.Infrastructure/ConfigMapster.API.Infrastructure.csproj", "ConfigMapster.API.Infrastructure/"]
COPY ["ConfigMapster.API.Persistence1/ConfigMapster.API.Persistence.csproj", "ConfigMapster.API.Persistence1/"]
RUN dotnet restore "./ConfigMapster/ConfigMapster.csproj"
COPY . .
WORKDIR "/src/ConfigMapster"
RUN dotnet build "./ConfigMapster.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConfigMapster.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigMapster.dll"]