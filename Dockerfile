# Multi-stage Dockerfile for Island CMS v2.0.0 (.NET 8.0)

# Runtime stage - Use latest .NET 8.0 runtime with latest security patches
FROM mcr.microsoft.com/dotnet/aspnet:8.0.12-alpine3.20 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install curl for health checks, update packages for security, and create non-root user
RUN apk update && apk upgrade && \
    apk add --no-cache curl ca-certificates && \
    apk del --purge apk-tools && \
    rm -rf /var/cache/apk/* && \
    addgroup --system --gid 1001 dotnet && \
    adduser --system --uid 1001 --ingroup dotnet --shell /bin/false dotnet

# Build stage - Use latest .NET 8.0 SDK with latest security patches
FROM mcr.microsoft.com/dotnet/sdk:8.0.12-alpine3.20 AS build
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

# Configure environment for container with security settings
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_ENABLE_DIAGNOSTICS=0
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Add security labels
LABEL maintainer="Island CMS Team"
LABEL description="Island CMS v2.0.0 - Secure container deployment"
LABEL version="2.0.0"
LABEL security.scan="enabled"

# Set strict file permissions
RUN chmod 755 /app && \
    find /app -type f -exec chmod 644 {} \; && \
    find /app -name "*.dll" -exec chmod 755 {} \;

ENTRYPOINT ["dotnet", "cms-mvc.dll"]