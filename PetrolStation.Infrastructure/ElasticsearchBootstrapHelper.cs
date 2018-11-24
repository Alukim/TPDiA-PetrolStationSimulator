using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using System;

namespace PetrolStation.Infrastructure
{
    public static class ElasticsearchBootstrapHelper
    {
        public static IServiceCollection AddElasticSearchConnection<TType>(this IServiceCollection services,
           IConfiguration configuration,
           Func<IServiceProvider, ConnectionSettings, ConnectionSettings> externalConfigurations = null)
        where TType : class, IEntity
        {
            services.AddElasticsearchSettings(configuration);
            services.AddSingleton<IElasticClient>(sp => new ElasticClient(sp.GetService<ConnectionSettings>()));
            services.AddSingleton<IndexNameProvider<TType>>(sp =>
                new IndexNameProvider<TType>(sp.GetService<IOptions<ElasticsearchSettings>>().Value.IndexName));
            services.AddScoped<ElasticsearchEntityRepository<TType>>();

            services.AddSingleton(sp =>
            {
                var node = new Uri(sp.GetService<IOptions<ElasticsearchSettings>>().Value.NodeUrl);
                var pool = new SingleNodeConnectionPool(node);

                var connectionSettings = new ConnectionSettings(
                        pool,
                        new HttpConnection(),
                        new SerializerFactory(settings => new ElasticsearchJsonNetSerializer(settings)))
                    .DisableDirectStreaming()
                    .PrettyJson()
                    .ThrowExceptions(true)
                    .DefaultIndex(sp.GetService<IOptions<ElasticsearchSettings>>().Value.IndexName);

                if (externalConfigurations != null)
                    connectionSettings = externalConfigurations(sp, connectionSettings);

                return connectionSettings;
            });

            return services;
        }

        public static IServiceCollection AddElasticsearchSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ElasticsearchSettings>(config.GetSection(nameof(ElasticsearchSettings)));
            return services;
        }
    }
}
