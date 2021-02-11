using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Logging;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Management.CloudFoundry;

namespace cms_mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
            .Build()
            .Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((context, builder) => builder.AddDynamicConsole())
                .AddCloudFoundryConfiguration()
                .AddCloudFoundryActuators()
                .AddConfigServer()
                .UseStartup<Startup>();

            // Load Kubernetes secrets file if available to load connection strings
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("secrets/appsettings.secrets.json", optional: true, reloadOnChange: false);
            });

            builder.ConfigureLogging((hostingContext, loggingBuilder) =>
            {
                loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                loggingBuilder.AddDynamicConsole();
            });
            return builder;
        }
    }
}
