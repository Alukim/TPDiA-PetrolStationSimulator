using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PetrolStation.View
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(LoadConfigFromJsonAndEnvironmentVariables)
                .UseStartup<Startup>();

        private static void LoadConfigFromJsonAndEnvironmentVariables(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var env = hostingContext.HostingEnvironment.EnvironmentName.ToLowerInvariant();

            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }
    }
}
