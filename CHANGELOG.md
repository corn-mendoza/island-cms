# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2024-12-06

### Added
- Docker support for containerized deployments
- GitHub Actions workflows for CI/CD pipeline
- Enhanced environment variable configuration support
- Container-optimized deployment configurations

### Changed
- **BREAKING CHANGE**: Upgraded from .NET Core 3.1 to .NET 8.0
- Updated Piranha CMS framework from v8.4.x to v11.1.1
- Updated all health check packages to v8.0.x for .NET 8.0 compatibility
- Updated Steeltoe packages from v3.2.3 to v3.2.8
- Replaced Microsoft.Extensions.Caching.Redis with Microsoft.Extensions.Caching.StackExchangeRedis v8.0.8
- Updated Microsoft.VisualStudio.Azure.Containers.Tools.Targets to v1.21.0
- All configuration now primarily handled through environment variables for container deployment
- Enhanced Docker support with multi-stage builds and optimized runtime images

### Security
- Updated all dependencies to latest versions with security patches
- Enhanced container security practices in Docker configuration

### Infrastructure
- Added comprehensive GitHub Actions workflows for build, test, and deployment
- Added multi-platform Docker build support
- Improved CI/CD pipeline with automated testing and deployment

### Documentation
- Updated README.md with .NET 8.0 deployment instructions
- Added comprehensive Docker deployment documentation
- Enhanced configuration and environment variable documentation