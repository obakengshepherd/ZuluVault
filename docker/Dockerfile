# Full Dockerfile with comments for ZuluVault API build
# This builds the .NET app in a multi-stage process for production.

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY src/ZuluVault.Api/*.csproj ./src/ZuluVault.Api/
COPY src/ZuluVault.Application/*.csproj ./src/ZuluVault.Application/
COPY src/ZuluVault.Domain/*.csproj ./src/ZuluVault.Domain/
COPY src/ZuluVault.Infrastructure/*.csproj ./src/ZuluVault.Infrastructure/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /app/src/ZuluVault.Api
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/ZuluVault.Api/out ./
ENTRYPOINT ["dotnet", "ZuluVault.Api.dll"]