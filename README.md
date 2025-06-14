# Island CMS v2.0.0

CMS Portal built using Piranha CMS framework v12.0.0 and optimized for container deployments. This project is based on the great work of the [Piranha CMS](https://piranhacms.org/) community.

## Overview
This application is built using **.NET 8.0** and the latest Piranha CMS framework v12.0.0. The application is designed for cloud-native deployments using containers with support for multiple storage backends (file, SQL Server, MySQL, PostgreSQL, SQLite) on Linux or Windows platforms.

## New in v2.0.0
- **Upgraded to .NET 8.0** for improved performance and security
- **Updated to Piranha CMS v12.0.0** with latest features and bug fixes  
- **Enhanced Docker support** with multi-stage builds and security improvements
- **Comprehensive GitHub Actions workflows** for CI/CD
- **Improved container security** with non-root user execution
- **Multi-platform container builds** (AMD64/ARM64)
- **Enhanced environment variable configuration** for cloud deployments

## Objectives

- Single .NET 8.0 codebase deployable to multiple scenarios (container, VM, server)
- Support for multiple database backends (file, SQL Server, MySQL, PostgreSQL, SQLite)
- Demonstrate 12-factor app patterns using native .NET and Steeltoe libraries
- Container-optimized with NFS volume mount support for content persistence
- Configuration-driven application with comprehensive environment variable support
- Support for Cloud Foundry, Kubernetes, and Azure Spring Cloud deployments
- Steeltoe v3 integration for cloud-native capabilities
- Built-in health checks and management endpoints

## Quick Start with Docker

### Using Docker Compose (Recommended)

```bash
# Clone the repository
git clone <repository-url>
cd island-cms

# Start the application with file-based storage
docker-compose up -d

# Or start with SQL Server
docker-compose --profile sqlserver up -d

# Or start with Redis caching
docker-compose --profile redis up -d
```

The application will be available at `http://localhost:8080`

### Using Docker CLI

```bash
# Build the image
docker build -t island-cms:latest .

# Run with file storage
docker run -d \
  --name island-cms \
  -p 8080:8080 \
  -v cms_data:/app/data \
  -v cms_uploads:/app/wwwroot/uploads \
  -e PIRANHA_DBTYPE=file \
  -e PIRANHA_HEALTHUI=true \
  island-cms:latest
```

## Configuration

### Environment Variables
All configuration can be managed through environment variables for container deployments:

| Variable | Description | Default | Valid Values |
|----------|-------------|---------|--------------|
| `PIRANHA_DBTYPE` | Database type | `file` | `file`, `sqlserver`, `mysql`, `postgres` |
| `PIRANHA_DBNAME` | Database filename (file type only) | `piranha.db` | Any valid filename |
| `PIRANHA_DBPATH` | Database path (file type only) | `.` | Any valid path |
| `PIRANHA_BASEPATH` | Media upload location | `wwwroot/uploads` | Any valid path |
| `PIRANHA_BASEURL` | Media URL location | `~/uploads/` | Any valid URL path |
| `PIRANHA_MEDIASTORE` | Media storage type | `file` | `file`, `azure` |
| `PIRANHA_SESSIONCACHE` | Enable session caching | `false` | `true`, `false` |
| `PIRANHA_REDISCACHE` | Use Redis for session cache | `false` | `true`, `false` |
| `PIRANHA_HEALTHUI` | Enable Health Check UI | `false` | `true`, `false` |
| `PIRANHA_DISCOVERY` | Enable service discovery | `false` | `true`, `false` |

### Database Configuration

#### Connection Strings
Configure database connections via environment variables or `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "piranha": "Server=sqlserver;Database=island_cms;UID=piranha_user;Password=password",
    "piranha-media": "DefaultEndpointsProtocol=https;AccountName=account;AccountKey=key;EndpointSuffix=core.windows.net"
  }
}
```

#### File Database (Default)
```bash
PIRANHA_DBTYPE=file
PIRANHA_DBNAME=piranha.db
PIRANHA_DBPATH=/app/data
```

#### SQL Server
```bash
PIRANHA_DBTYPE=sqlserver
ConnectionStrings__piranha="Server=sqlserver;Database=cms;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
```

#### MySQL
```bash
PIRANHA_DBTYPE=mysql
ConnectionStrings__piranha="Server=mysql;Database=cms;Uid=root;Pwd=password;"
```

#### PostgreSQL
```bash
PIRANHA_DBTYPE=postgres
ConnectionStrings__piranha="Host=postgres;Database=cms;Username=postgres;Password=password"
```

### Media Storage

#### File Storage (Default)
```bash
PIRANHA_MEDIASTORE=file
PIRANHA_BASEPATH=/app/wwwroot/uploads
PIRANHA_BASEURL=~/uploads/
```

#### Azure Blob Storage
```bash
PIRANHA_MEDIASTORE=azure
ConnectionStrings__piranha-media="DefaultEndpointsProtocol=https;AccountName=storage;AccountKey=key;"
```

## Container Deployment

### Production Deployment

The application includes optimized Docker configuration for production:

- **Multi-stage builds** for smaller image sizes
- **Non-root user execution** for enhanced security
- **Health checks** for container orchestration
- **Multi-platform support** (AMD64/ARM64)
- **Optimized for container registries**

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: island-cms
spec:
  replicas: 2
  selector:
    matchLabels:
      app: island-cms
  template:
    metadata:
      labels:
        app: island-cms
    spec:
      containers:
      - name: island-cms
        image: ghcr.io/your-org/island-cms:latest
        ports:
        - containerPort: 8080
        env:
        - name: PIRANHA_DBTYPE
          value: "sqlserver"
        - name: PIRANHA_HEALTHUI
          value: "true"
        - name: ConnectionStrings__piranha
          valueFrom:
            secretKeyRef:
              name: database-connection
              key: connection-string
        volumeMounts:
        - name: uploads
          mountPath: /app/wwwroot/uploads
        - name: data
          mountPath: /app/data
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 60
          periodSeconds: 30
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
      volumes:
      - name: uploads
        persistentVolumeClaim:
          claimName: cms-uploads
      - name: data
        persistentVolumeClaim:
          claimName: cms-data
```

### Volume Mounts for Persistence

For persistent deployments, mount volumes for:

- **Database files**: `/app/data` (when using file database)
- **Media uploads**: `/app/wwwroot/uploads`
- **Secrets**: `/app/secrets` (optional)

## Cloud Platform Support

### Cloud Foundry
Use the included `cms_manifest.yml` for basic deployments:

```bash
cf push -f cms_manifest.yml
```

### Kubernetes
Full Kubernetes support with:
- ConfigMaps for configuration
- Secrets for sensitive data  
- Persistent volumes for data
- Health checks and readiness probes

### Azure Spring Cloud
Native integration with Azure Spring Cloud Services for configuration management and service discovery.

## Development

### Prerequisites
- .NET 8.0 SDK
- Docker (optional)
- Database server (optional, file storage works out of box)

### Local Development
```bash
# Restore packages
dotnet restore

# Run the application
dotnet run

# Or using Docker
docker-compose -f docker-compose.yml up
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

### Building for Production
```bash
# Build release version
dotnet build --configuration Release

# Publish self-contained
dotnet publish --configuration Release --runtime linux-x64 --self-contained
```

## Health Monitoring

### Health Checks
Built-in health checks for:
- Database connectivity
- External storage (Azure Blob)
- Redis cache (when enabled)
- Application responsiveness

Access health checks:
- **Health status**: `/health`
- **Health UI**: `/healthchecks-ui` (when `PIRANHA_HEALTHUI=true`)

### Monitoring and Observability

#### Steeltoe Management Endpoints
Available management endpoints:
- `/cloudfoundryapplication/info` - Application information
- `/cloudfoundryapplication/health` - Health status
- `/cloudfoundryapplication/metrics` - Application metrics
- `/cloudfoundryapplication/loggers` - Dynamic logging control

#### Distributed Tracing
Integrated OpenTelemetry tracing for monitoring request flows across services.

#### Dynamic Logging
Serilog dynamic logging with runtime log level configuration support.

## Security

### Container Security
- **Non-root user**: Application runs as non-privileged user
- **Read-only filesystem**: Minimal write permissions
- **Security scanning**: Automated vulnerability scanning in CI/CD
- **Minimal attack surface**: Alpine-based images with minimal packages

### Application Security
- **Identity integration**: ASP.NET Core Identity with multiple providers
- **HTTPS enforcement**: Configurable HTTPS redirection
- **Content Security Policy**: Headers for XSS protection
- **Secure headers**: Security-focused HTTP headers

## CI/CD

The project includes comprehensive GitHub Actions workflows:

### Continuous Integration
- **Build and test** on multiple platforms
- **Code coverage** reporting
- **Security scanning** with Trivy
- **Multi-platform container builds**

### Release Management
- **Automated releases** with GitHub Releases
- **Cross-platform binaries** (Windows, Linux, macOS)
- **Container images** pushed to GitHub Container Registry
- **Release notes** generation

## Migration from v1.x

### Breaking Changes in v2.0.0
- **.NET Core 3.1 → .NET 8.0**: Update your runtime environment
- **Piranha CMS v8.x → v12.0**: Review API changes in Piranha documentation
- **Container port change**: Default port changed from 80/443 to 8080/8081
- **Package updates**: All dependencies updated to latest versions

### Migration Steps
1. Update your .NET runtime to 8.0
2. Update container base images if using custom builds
3. Review environment variable configurations
4. Test thoroughly in non-production environment
5. Update deployment manifests for new port numbers

## Troubleshooting

### Common Issues

#### Database Connection Issues
- Verify connection strings are correctly formatted
- Ensure database server is accessible from container
- Check firewall rules and network policies

#### Volume Mount Issues
- Ensure proper permissions on mount points
- Verify volume paths exist and are writable
- Check SELinux/AppArmor policies if applicable

#### Performance Issues
- Enable Redis caching for better session performance
- Use external database instead of file storage for production
- Monitor resource usage and scale accordingly

### Logs and Debugging
Enable detailed logging:
```bash
ASPNETCORE_ENVIRONMENT=Development
Logging__LogLevel__Default=Debug
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project maintains the same license as the original Island CMS project.

## Support

- **Issues**: Report issues on GitHub
- **Documentation**: See [Piranha CMS Documentation](https://piranhacms.org/docs)
- **Community**: Join the Piranha CMS community discussions

## Acknowledgments

- **Piranha CMS Team**: For the excellent CMS framework
- **Steeltoe Team**: For cloud-native .NET capabilities
- **Community Contributors**: For ongoing improvements and feedback