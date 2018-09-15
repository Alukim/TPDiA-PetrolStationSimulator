using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using PetrolStation.Infrastructure;
using PetrolStation.Simulator.Contracts.Events;
using PetrolStation.View.Entities;
using PetrolStation.View.Factories;
using PetrolStation.View.Handlers;
using System;

namespace PetrolStation.View
{
    public class Startup
    {
        public readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
            => this.configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<CallContext>(x => CallContext.CreateEmpty());

            services.AddSingleton<KafkaTopicProvider>();
            services.AddSingleton<JsonSerializerSettingsProvider>();
            services.AddSingleton<EventDispatcher>();
            services.AddScoped<EventBus>();

            services.AddElasticSearchConnection<Entities.PetrolStation>(configuration);

            services.Configure<GlobalSettings>(configuration.GetSection("GlobalSettings"));

            services.AddLogging(x =>
            {
                x.AddConfiguration(configuration.GetSection("Logging"))
                    .AddConsole()
                    .AddDebug();
            });

            services.AddHostedService<KafkaConsumer>();

            services.AddTransient<IEventHandler<PetrolStationCreated>, PetrolStationEventHandlers>();
            services.AddTransient<IEventHandler<NozzleUsed>, PetrolStationEventHandlers>();
            services.AddTransient<IEventHandler<PetrolStolenByCustomer>, PetrolStationEventHandlers>();
            services.AddTransient<IEventHandler<TankRefueled>, PetrolStationEventHandlers>();

            services.AddScoped<PetrolStationReportFactory>();

            services.AddSignalR();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            InitializeIndex(app.ApplicationServices);
            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRHub>("/petrolHub");
            });
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        private void InitializeIndex(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var elasticClient = scope.ServiceProvider.GetService<IElasticClient>();
                var indexNameProvider = scope.ServiceProvider.GetService<IndexNameProvider<Entities.PetrolStation>>();

                if (!elasticClient.IndexExists(indexNameProvider.IndexName).Exists)
                {
                    var elasticReasponse = elasticClient.CreateIndex(indexNameProvider.IndexName, x =>
                        x.Settings(s =>
                                s.Setting("index.mapper.dynamic", true)
                                    .NumberOfShards(1))
                            .Mappings(m => m.Map<Entities.PetrolStation>(md =>
                                md.AutoMap()
                                    .AutoMap(typeof(Tank))
                                    .AutoMap(typeof(Nozzle))
                                    .AutoMap(typeof(PetrolStationReport)))));

                    if (!elasticReasponse.IsValid)
                        throw new Exception("Error on elastic index initialization");
                }
            }
        }
    }
}
