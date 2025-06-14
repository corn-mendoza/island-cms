# Multi-stage Dockerfile for Island CMS v2.0.0 (.NET 8.0)

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create non-root user for security
RUN addgroup --system --gid 1001 dotnet \
    && adduser --system --uid 1001 --ingroup dotnet --shell /bin/false dotnet

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["cms-mvc.csproj", "./"]
RUN dotnet restore "cms-mvc.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "cms-mvc.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "cms-mvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish --chown=dotnet:dotnet /app/publish .

# Create necessary directories with proper permissions
RUN mkdir -p /app/wwwroot/uploads /app/data /app/secrets \
    && chown -R dotnet:dotnet /app

# Switch to non-root user
USER dotnet

# Configure environment for container
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_ENABLE_DIAGNOSTICS=0

ENTRYPOINT ["dotnet", "cms-mvc.dll"]