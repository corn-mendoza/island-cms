# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2024-12-06

### Added
- Docker support for containerized deployments with multi-stage builds
- GitHub Actions workflows for comprehensive CI/CD pipeline
- Enhanced environment variable configuration support for cloud deployments
- Container-optimized deployment configurations with security best practices
- PostComment model for concrete Comment implementation (Piranha CMS v12.0.0 compatibility)
- Comprehensive input validation with data annotations
- Health check endpoints for container orchestration
- Multi-platform container builds (AMD64/ARM64)
- Automated security scanning with Trivy vulnerability scanner
- Cross-platform release binaries for Windows, Linux, and macOS

### Changed
- **BREAKING CHANGE**: Upgraded from .NET Core 3.1 to .NET 8.0
- **BREAKING CHANGE**: Updated Piranha CMS framework from v8.4.x to v12.0.0
- **BREAKING CHANGE**: Container port changed from 80/443 to 8080/8081
- Updated all health check packages to latest .NET 8.0 compatible versions
- Updated Steeltoe packages from v3.2.3 to v3.2.8
- Replaced Microsoft.Extensions.Caching.Redis with Microsoft.Extensions.Caching.StackExchangeRedis v8.0.8
- Updated Microsoft.VisualStudio.Azure.Containers.Tools.Targets to v1.21.0
- All configuration now primarily handled through environment variables for container deployment
- Enhanced Docker support with Alpine Linux base images for smaller footprint
- Improved error handling and user feedback throughout the application

### Fixed
- Comment instantiation issues due to abstract Comment class in Piranha CMS v12.0.0
- Program.cs hosting configuration for .NET 8.0 compatibility
- MySQL ServerVersion configuration for Entity Framework Core with proper null handling
- Missing using statements for ASP.NET Core hosting
- Null reference handling in Comment creation and IP address resolution
- Kubernetes configuration conditional loading to prevent startup failures
- Health endpoint mapping for proper Docker health checks
- Database connection string validation with meaningful error messages
- GitHub Actions security scan image reference issues
- CodeQL action deprecation warnings (updated from v2 to v3)
- Missing SARIF file handling in security scanning pipeline
- Docker health check configuration with curl installation
- Conditional logic in release workflow for Windows archive creation

### Security
- Updated all dependencies to latest versions with security patches
- Enhanced container security practices with non-root user execution
- Automated vulnerability scanning integrated into CI/CD pipeline
- Input validation and sanitization for all user inputs
- Proper error handling to prevent information disclosure
- Container security scanning with severity filtering (CRITICAL/HIGH)
- Alpine Linux base images for minimal attack surface

### Infrastructure
- Added comprehensive GitHub Actions workflows for build, test, security scan, and deployment
- Added multi-platform Docker build support with caching optimization
- Improved CI/CD pipeline with automated testing, security scanning, and deployment
- Container registry integration with GitHub Container Registry
- Automated release management with cross-platform binaries
- Health check integration for Kubernetes and Docker Compose deployments
- Robust fallback strategies for CI/CD pipeline reliability

### Documentation
- Complete rewrite of README.md with .NET 8.0 deployment instructions
- Added comprehensive Docker deployment documentation with examples
- Enhanced configuration and environment variable documentation
- Added migration guide from v1.x to v2.0.0
- Included troubleshooting section for common deployment issues
- Added Kubernetes deployment examples and best practices
- Documented security features and container hardening practices

### Development Experience
- Added comprehensive data validation attributes to models
- Improved error messages and debugging information
- Enhanced development workflow with proper health endpoints
- Better logging and monitoring capabilities
- Streamlined container development with docker-compose support