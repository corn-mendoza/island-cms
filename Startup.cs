using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.Data.EF.SQLServer;
using Piranha.Manager.Editor;
using System;

namespace cms_mvc
{
    public class Startup
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration">The current configuration</param>
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string _basePath = null;
            string _envPath = Environment.GetEnvironmentVariable("PIRANHA_BASEPATH");
            if (!string.IsNullOrEmpty(_envPath))
            {
                _basePath = _envPath;
            }
            string _baseUrl = null;
            string _envUrl = Environment.GetEnvironmentVariable("PIRANHA_BASEURL");
            if (!string.IsNullOrEmpty(_envUrl))
            {
                _baseUrl = _envUrl;
            }
            // Service setup
            services.AddPiranha(options =>
            {
                options.AddRazorRuntimeCompilation = true;

                options.UseFileStorage(basePath: _basePath, baseUrl: _baseUrl, naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
                options.UseImageSharp();
                options.UseManager();
                options.UseTinyMCE();
                options.UseMemoryCache();

                options.UseEF<SQLServerDb>(db =>
                    db.UseSqlServer(_config.GetConnectionString("piranha")));
                options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                    db.UseSqlServer(_config.GetConnectionString("piranha")));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApi api)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            // Middleware setup
            app.UsePiranha(options => {
                options.UseManager();
                options.UseTinyMCE();
                options.UseIdentity();
            });
        }
    }
}
