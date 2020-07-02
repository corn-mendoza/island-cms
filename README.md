# Island CMS
CMS Portal built using Piranha CMS frameworks and modified for Container deployments

## Overview
This application is built using .NET Core 3.1 and CMS frameworks developed by Piranha CMS Open Source project. The application is designed to use an NFS mount to store media and SQL Server to store application data.

## Objectives

- Use NFS volume mounts with .NET Core application deployments to persist content
- Implement a simple configuration based application options support for different deployment options

## Installation

### Available Configuration Environment Variables
By default, the environment variables will always override configuration settings located in the appsettings.json file. All of the settings below can also be configured in settings file.

- PIRANHA_DBTYPE - Database type for application data
	- Valid Values: file (default) | sqlserver | mysql | postgres
	- Parameters: Connection String provided in appsettings.json or secrets
- PIRANHA_DBNAME - Database filename for database file
	- Valid Values: string (default = piranha.db)
	- Note: Only valid for 'file' DBTYPE
- PIRANHA_DBPATH - Database path for database file
	- Valid Values: string (default = .)
	- Note: Only valid for 'file' DBTYPE
- PIRANHA_BASEPATH - Upload location for media files
	- Valid Values: string (default = wwwroot/uploads)
- PIRANHA_BASEURL - URL location for media files
	- Valid Values: string (default = ~/uploads/)
- PIRANHA_MEDIASTORE - Media storage type 
	- Valid Values: file (default) | azure
	- Parameters: Connection String provided in appsettings.json or secrets


### Available Configuration via appsettings.json
The application can be configured using the standard .NET Core configuration files.

Example appsettings.json entries:

		"piranha": {
		  "DatabaseType": "file",
		  "DatabaseFilename": "piranha.db",
		  "DatabasePath": ".",
		  "BasePath": "wwwroot/uploads",
		  "BaseUrl": "~/uploads/",
		  "MediaStorageType": "file"
		},
		"ConnectionStrings": {
		  "piranha": "Server=sqlserver;Database=island_cms;UID=piranha_user;Password=password",
		  "blobstorage": "DefaultEndpointsProtocol=https;AccountName=;AccountKey=;EndpointSuffix="
		}


### Secrets Support
This application is designed to support loading an optional secrets file located in the secrets sub-folder. Additional settings can be injected by providing an `appsettings.secrets.json` file placed in the secrets folder.
