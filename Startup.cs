using System;
using cms_mvc.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Piranha;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.AttributeBuilder;
using Piranha.Data.EF.MySql;
using Piranha.Data.EF.PostgreSql;
using Piranha.Data.EF.SQLite;
using Piranha.Data.EF.SQLServer;
using Piranha.Manager.Editor;
using Pivotal.Helper;
using Steeltoe.Connector.Redis;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Discovery.Kubernetes;
using Steeltoe.Management.Tracing;



namespace cms_mvc
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private PiranhaOptions _appOptions;

        public IConfiguration Configuration
        {
            get
            {
                return _config;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration">The current configuration</param>
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            // Add Distributed tracing
            services.AddDistributedTracing();

            _appOptions = _config.GetSection("piranha").Get<PiranhaOptions>();

            // Enabling session caching for the admin functionality is not recommended - ideally, a second singleton container would be deployed to allow for management functions
            if (_appOptions.EnableSessionCache)
            {
                // Add Session Caching function
                if (_appOptions.EnableRedisCache)
                {
                    services.AddDistributedRedisCache(_config);
                }
                else
                {
                    services.AddDistributedMemoryCache();
                }
                services.AddSession();
            }

            if (_appOptions.EnableDiscoveryClient)
            {
                // Add Discovery Client
                services.AddDiscoveryClient();
                services.AddServiceDiscovery(options => options
                    .UseEureka()
                    .UseKubernetes()
                    .UseConsul());
            }

            if (_appOptions.EnableHealthUI)
            {
                services
                    .AddHealthChecksUI()
                    .AddInMemoryStorage();
            }

            // Service setup
            services.AddPiranha(options =>
            {
                options.AddRazorRuntimeCompilation = true;

                if (_appOptions.UseFileStorage)
                    options.UseFileStorage(basePath: _appOptions.BasePath, baseUrl: _appOptions.BaseUrl, naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
                else
                {
                    var _blob_connect = Environment.GetEnvironmentVariable("PIRANHA_MEDIA_CONNECTION");

                    CFEnvironmentVariables _cfEnv = new CFEnvironmentVariables();
                    var _azure_bind = _cfEnv.getAzureStorageCredentials("azure-storage", "piranha-media");
                    if (_azure_bind != null)
                        _blob_connect = _azure_bind.ConnectionString;

                    if (string.IsNullOrEmpty(_blob_connect))
                        _blob_connect = _config.GetConnectionString("piranha-media");

                    if (!string.IsNullOrEmpty(_blob_connect))
                    {
                        options.UseBlobStorage(_blob_connect);
                        services.AddHealthChecks()
                            .AddAzureBlobStorage(_blob_connect);
                    }
                }

                options.UseImageSharp();
                options.UseManager();
                options.UseTinyMCE();
                options.UseMemoryCache();

                switch (_appOptions.DatabaseType)
                {
                    case "mysql":
                        var mysqlConnectionString = _config.GetConnectionString("piranha");
                        if (string.IsNullOrEmpty(mysqlConnectionString))
                            throw new InvalidOperationException("MySQL connection string 'piranha' is required when using MySQL database type");
                        
                        var serverVersion = ServerVersion.AutoDetect(mysqlConnectionString);
                        options.UseEF<MySqlDb>(db =>
                            db.UseMySql(mysqlConnectionString, serverVersion));
                        options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                            db.UseMySql(mysqlConnectionString, serverVersion));
                        services.AddHealthChecks()
                            .AddMySql(mysqlConnectionString);
                        break;

                    case "postgres":
                        var postgresConnectionString = _config.GetConnectionString("piranha");
                        if (string.IsNullOrEmpty(postgresConnectionString))
                            throw new InvalidOperationException("PostgreSQL connection string 'piranha' is required when using PostgreSQL database type");
                        
                        options.UseEF<PostgreSqlDb>(db =>
                            db.UseNpgsql(postgresConnectionString));
                        options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                            db.UseNpgsql(postgresConnectionString));
                        services.AddHealthChecks()
                            .AddNpgSql(postgresConnectionString);
                        break;

                    case "sqlserver":
                        var sqlServerConnectionString = _config.GetConnectionString("piranha");
                        if (string.IsNullOrEmpty(sqlServerConnectionString))
                            throw new InvalidOperationException("SQL Server connection string 'piranha' is required when using SQL Server database type");
                        
                        options.UseEF<SQLServerDb>(db =>
                            db.UseSqlServer(sqlServerConnectionString));
                        options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                            db.UseSqlServer(sqlServerConnectionString));
                        services.AddHealthChecks()
                            .AddSqlServer(sqlServerConnectionString);
                        break;

                    default:
                        options.UseEF<SQLiteDb> (db =>
                            db.UseSqlite("Filename="+_appOptions.DatabaseFilePath));
                        options.UseIdentityWithSeed<IdentitySQLiteDb>(db =>
                            db.UseSqlite("Filename="+_appOptions.DatabaseFilePath));
                        services.AddHealthChecks()
                            .AddSqlite("Filename=" + _appOptions.DatabaseFilePath);
                        break;

                }
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApi api)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //if (_appOptions.EnableDiscoveryClient)
            //{
            //    app.UseDiscoveryClient();
            //}

            if (_appOptions.EnableSessionCache)
            {
                app.UseSession();
            }

            // Initialize Piranha
            App.Init(api);

            // Build content types
            new ContentTypeBuilder(api)
                .AddAssembly(typeof(Startup).Assembly)
                .Build()
                .DeleteOrphans();

            // Configure Tiny MCE
            EditorConfig.FromFile("editorconfig.json");

            // Configure health check endpoints
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                // Basic health check endpoint
                endpoints.MapHealthChecks("/health");
                
                // Health checks UI if enabled
                if (_appOptions.EnableHealthUI)
                {
                    endpoints.MapHealthChecksUI();
                }
            });

            // Middleware setup
            app.UsePiranha(options => {
                options.UseManager();
                options.UseTinyMCE();
                options.UseIdentity();
            });
        }
    }
}
