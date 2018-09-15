using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using PetrolStation.Infrastructure;
using PetrolStation.Simulator.Factories;
using System;
using System.IO;
using System.Threading.Tasks;
using PetrolStation.Simulator;

namespace PetrolStationSimulator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, serviceCollection) =>
                {
                    serviceCollection.AddScoped<CallContext>(x => CallContext.CreateEmpty());

                    serviceCollection.AddSingleton<KafkaTopicProvider>();
                    serviceCollection.AddSingleton<JsonSerializerSettingsProvider>();
                    serviceCollection.AddScoped<KafkaProducer>();
                    serviceCollection.AddElasticSearchConnection<PetrolStation.Simulator.Entities.PetrolStation>(hostContext.Configuration);

                    serviceCollection.Configure<GlobalSettings>(hostContext.Configuration.GetSection("GlobalSettings"));

                    serviceCollection.AddLogging(x =>
                    {
                        x.AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                            .AddConsole()
                            .AddDebug();
                    });

                    serviceCollection.AddScoped<PetrolStationFactory>();

                    serviceCollection.AddHostedService<Application>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                });

            await builder.RunConsoleAsync();
        }
    }
}
