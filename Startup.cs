using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Steeltoe.Management.CloudFoundry;
using Microsoft.Extensions.Logging;
using cms_mvc.Models;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.Manager.Editor;
using Piranha.Azure;
using Piranha.Data.EF.MySql;
using Piranha.Data.EF.SQLite;
using Piranha.Data.EF.SQLServer;
using Piranha.Data.EF.PostgreSql;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.AspNetCore.Identity.MySQL;
using Piranha.AspNetCore.Identity.PostgreSQL;
using Steeltoe.Connector.Redis;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Kubernetes;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Discovery.Consul;
using Steeltoe.Management.Tracing;

namespace cms_mvc
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private PiranhaOptions _appOptions;

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
            services.AddDistributedTracing(_config, builder => builder.UseZipkinWithTraceOptions(services));

            _appOptions = _config.GetSection("piranha").Get<PiranhaOptions>();

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

            if (_appOptions.EnableDiscoveryClient)
            {
                // Add Discovery Client
                services.AddDiscoveryClient();
                services.AddServiceDiscovery(options => options
                    .UseEureka()
                    .UseKubernetes()
                    .UseConsul());
            }

            // Service setup
            services.AddPiranha(options =>
            {
                options.AddRazorRuntimeCompilation = true;

                if (_appOptions.UseFileStorage)
                    options.UseFileStorage(basePath: _appOptions.BasePath, baseUrl: _appOptions.BaseUrl, naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
                else
                    options.UseBlobStorage(_config.GetConnectionString("piranha-media"));

                options.UseImageSharp();
                options.UseManager();
                options.UseTinyMCE();
                options.UseMemoryCache();

                switch (_appOptions.DatabaseType)
                {
                    case "mysql":
                        options.UseEF<MySqlDb>(db =>
                            db.UseMySql(_config.GetConnectionString("piranha")));
                        options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                            db.UseMySql(_config.GetConnectionString("piranha")));
                        break;

                    case "postgres":
                        options.UseEF<PostgreSqlDb>(db =>
                            db.UseNpgsql(_config.GetConnectionString("piranha")));
                        options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                            db.UseNpgsql(_config.GetConnectionString("piranha")));
                        break;

                    case "sqlserver":
                        options.UseEF<SQLServerDb>(db =>
                            db.UseSqlServer(_config.GetConnectionString("piranha")));
                        options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                            db.UseSqlServer(_config.GetConnectionString("piranha")));
                        break;

                    default:
                        options.UseEF<SQLiteDb> (db =>
                            db.UseSqlite("Filename="+_appOptions.DatabaseFilePath));
                        options.UseIdentityWithSeed<IdentitySQLiteDb>(db =>
                            db.UseSqlite("Filename="+_appOptions.DatabaseFilePath));
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

            if (_appOptions.EnableDiscoveryClient)
            {
                app.UseDiscoveryClient();
            }

            app.UseSession();

            // Initialize Piranha
            App.Init(api);

            // Build content types
            new ContentTypeBuilder(api)
                .AddAssembly(typeof(Startup).Assembly)
                .Build()
                .DeleteOrphans();

            // Configure Tiny MCE
            EditorConfig.FromFile("editorconfig.json");

            // Middleware setup
            app.UsePiranha(options => {
                options.UseManager();
                options.UseTinyMCE();
                options.UseIdentity();
            });
        }
    }
}
