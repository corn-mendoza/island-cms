using Microsoft.Azure.SpringCloud.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.Kubernetes;
using Steeltoe.Extensions.Logging.DynamicSerilog;
using Steeltoe.Management.Endpoint;
using Steeltoe.Connector;

namespace cms_mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    // Add Serilog Dynamic Logger
                    loggingBuilder.AddDynamicSerilog();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Load optional secrets file if available to load connection strings
                    config.AddJsonFile("secrets/appsettings.secrets.json", optional: true, reloadOnChange: false);

                    // Load all the connection strings for bound services via Steeltoe connectors which benefits TAS developers
                    //    - Also supports Azure Service Broker service bindings via STv3
                    //    - This approach allows for multiple database connections for the same database type using connection string names
                    config.AddConnectionStrings();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        // For loading when on cloud foundry
                        .AddAllActuators()
                        
                        // For loading when on azure spring cloud
                        .UseAzureSpringCloudService()
                        
                        // Add Config Server if available
                        .AddConfigServer()
                        
                        // Load Config values for K8s
                        .AddKubernetesConfiguration()
                        
                        .UseStartup<Startup>();
                });
    }
}
