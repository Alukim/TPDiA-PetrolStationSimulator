using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using PetrolStation.Infrastructure;
using PetrolStation.Simulator.Entities;
using PetrolStation.Simulator.Factories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetrolStation.Simulator
{
    public class Application : BackgroundService
    {
        private const double minute_interval = 3;

        private readonly KafkaProducer kafkaProducer;
        private readonly IOptions<GlobalSettings> globalSettings;
        private readonly ILogger<Application> logger;
        private readonly ElasticsearchEntityRepository<Entities.PetrolStation> repository;
        private readonly IElasticClient elasticClient;
        private readonly IndexNameProvider<Entities.PetrolStation> indexNameProvider;
        private readonly PetrolStationFactory petrolStationFactory;

        public Application(KafkaProducer kafkaProducer, IOptions<GlobalSettings> globalSettings, ILogger<Application> logger,
            ElasticsearchEntityRepository<Entities.PetrolStation> repository, IElasticClient elasticClient,
            IndexNameProvider<Entities.PetrolStation> indexNameProvider, PetrolStationFactory petrolStationFactory)
        {
            this.kafkaProducer = kafkaProducer;
            this.globalSettings = globalSettings;
            this.logger = logger;
            this.repository = repository;
            this.elasticClient = elasticClient;
            this.indexNameProvider = indexNameProvider;
            this.petrolStationFactory = petrolStationFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InitializeIndex();
            var petrolStation = await CreateStationIfDoesNotExist();
            logger.LogDebug("Petrol station simulator started.");
            int i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                await SimulatePetrolStationWorkCycle(petrolStation);
            }
             
            logger.LogDebug("Petrol station simulator exited.");
        }

        private async Task SimulatePetrolStationWorkCycle(Entities.PetrolStation petrolStation)
        {
            petrolStation.ChangeCurrentBusinessDate(minute_interval);
            petrolStation.SimulateActivity();
            var orders = petrolStation.GetOrders();
            petrolStation.ChangeCurrentBusinessDate(minute_interval);
            var supplies = SupplyFactory.CreateFor(orders, petrolStation.CurrentDateTime);
            petrolStation.RefuelTanks(supplies);

            await repository.UpdateAsync(petrolStation);
            await SendEvents(petrolStation);
        }


        private void InitializeIndex()
        {
            if (!elasticClient.IndexExists(indexNameProvider.IndexName).Exists)
            {
                var elasticReasponse = elasticClient.CreateIndex(indexNameProvider.IndexName, x =>
                    x.Settings(s =>
                            s.Setting("index.mapper.dynamic", true)
                                .NumberOfShards(1))
                        .Mappings(m => m.Map<Entities.PetrolStation>(md =>
                            md.AutoMap()
                                .AutoMap(typeof(Nozzle))
                                .Properties(p => p.Nested<Tank>(n => n
                                    .Name(na => na.Tanks)
                                    .AutoMap()
                                    .Properties(pp => pp.Object<RefuelOrder>(o => o
                                        .Name(na => na.RefuelOrder)
                                        .AutoMap())))
                                    )
                            )
                        ));

                if (!elasticReasponse.IsValid)
                    throw new Exception("Error on elastic index initialization");
            }
        }

        private async Task<Entities.PetrolStation> CreateStationIfDoesNotExist()
        {
            var petrolStation = await repository.FindAsync(globalSettings.Value.ServiceId);

            if (petrolStation != null)
                return petrolStation;

            petrolStation = petrolStationFactory.Create();

            await repository.AddAsync(petrolStation);

            await SendEvents(petrolStation);

            return petrolStation;
        }

        private async Task SendEvents(Entities.PetrolStation petrolStation)
        {
            for(int i = 0; i < petrolStation.EventsToSend.Count; i++)
            {
                var @event = petrolStation.EventsToSend.Dequeue();
                await kafkaProducer.ProduceEvent(@event);
            }
        }
    }
}
