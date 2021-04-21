using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.SpringCloud.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.Kubernetes;
using Steeltoe.Extensions.Logging.DynamicSerilog;
using Steeltoe.Management.CloudFoundry;
using Steeltoe.Connector;

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
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    //loggingBuilder.AddDynamicConsole();

                    // Add Serilog Dynamic Logger
                    loggingBuilder.AddDynamicSerilog();
                })

                // For loading when on cloud foundry
                .AddCloudFoundryConfiguration()
                .AddCloudFoundryActuators()

                // For loading when on azure spring cloud
                .UseAzureSpringCloudService()

                // Add Config Server if available
                .AddConfigServer()

                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Load optional secrets file if available to load connection strings
                    config.AddJsonFile("secrets/appsettings.secrets.json", optional: true, reloadOnChange: false);

                    // Load all the connection strings for bound services via Steeltoe connectors which benefits TAS developers
                    //    - Also supports Azure Service Broker service bindings via STv3
                    //    - This approach allows for multiple database connections for the same database type using connection string names
                    config.AddConnectionStrings();
                });

                // Load Config values for K8s
                try
                {
                    builder.AddKubernetesConfiguration();
                }
                catch
                {
                }

                builder.UseStartup<Startup>();

            return builder;
        }
    }
}
