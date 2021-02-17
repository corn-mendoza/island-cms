using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.SpringCloud.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Logging.DynamicSerilog;
using Steeltoe.Management.CloudFoundry;

#if __USE_DISCOVERY_CLIENT__
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Kubernetes;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Discovery.Consul;
#endif

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

                // Load Kubernetes secrets file if available to load connection strings
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("secrets/appsettings.secrets.json", optional: true, reloadOnChange: false);
                })

#if __USE_DISCOVERY_CLIENT__
                // Add Discovery Client
                .AddDiscoveryClient()
                .AddServiceDiscovery(options => options
                    .UseEureka()
                    .UseKubernetes()
                    .UseConsul())
#endif

                .UseStartup<Startup>();

            return builder;
        }
    }
}
