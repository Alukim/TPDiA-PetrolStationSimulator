using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PetrolStation.Infrastructure;
using PetrolStation.Simulator.Contracts.Events;
using PetrolStation.View.Entities;
using PetrolStation.View.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.View.Handlers
{
    public class PetrolStationEventHandlers :
        IEventHandler<PetrolStationCreated>,
        IEventHandler<NozzleUsed>,
        IEventHandler<PetrolStolenByCustomer>,
        IEventHandler<TankRefueled>
    {
        private readonly ILogger<PetrolStationEventHandlers> logger;
        private readonly ElasticsearchEntityRepository<Entities.PetrolStation> repository;
        private readonly CallContext callContext;
        private readonly IHubContext<SignalRHub> hub;
        private readonly PetrolStationReportFactory factory;

        public PetrolStationEventHandlers(ILogger<PetrolStationEventHandlers> logger, ElasticsearchEntityRepository<Entities.PetrolStation> repository, CallContext callContext, IHubContext<SignalRHub> hub, PetrolStationReportFactory factory)
        {
            this.logger = logger;
            this.repository = repository;
            this.callContext = callContext;
            this.hub = hub;
            this.factory = factory;
        }

        public async Task Handle(PetrolStationCreated @event)
        {
            logger.LogDebug($"Petrol station created event received. Id: {@event.Id}, Name: {@event.Name}");

            var petrolStation = await repository.FindAsync(@event.Id);

            if (petrolStation != null)
                return;

            var tanks = new List<Tank>();

            foreach(var item in @event.Tanks)
            {
                var nozzles = @event.Nozzles?.Where(x => x.TankId == item.Id).Select(x => new Nozzle
                {
                    Id = x.Id,
                    LastStolenPetrolAmount = x.LastStolenPetrolAmount,
                    LastTransactionVolume = x.LastTransactionVolume,
                    TotalPetrolAmount = 0,
                    TotalStolenPetrolAmount = x.TotalStolenPetrolAmount
                });

                var tank = new Tank
                {
                    Id = item.Id,
                    CurrentVolume = item.CurrentVolume,
                    MaximumVolume = item.MaximumVolume,
                    PetrolTemperature = item.PetrolTemperature,
                    TankHigh = item.TankHigh,
                    Nozzles = nozzles.ToList()
                };

                tanks.Add(tank);
            }

            petrolStation = new Entities.PetrolStation()
            {
                Id = @event.Id,
                Name = @event.Name,
                Tanks = tanks.ToList()
            };

            await repository.AddAsync(petrolStation);

            await hub.Clients.All.SendAsync("NewStation");

            logger.LogDebug($"Petrol station created event received handling ended.");
        }

        public async Task Handle(NozzleUsed @event)
        {
            logger.LogDebug($"Nozzle used event received. Id: {@event.Id}");

            var petrolStation = await repository.GetAsync(callContext.PetrolStationId.Value);

            petrolStation.Time = @event.TimeStamp;

            var tank = petrolStation.Tanks.SingleOrDefault(x => x.Nozzles.Any(z => z.Id == @event.Id));

            var nozzle = tank.Nozzles.Single(z => z.Id == @event.Id);
            nozzle.LastTransactionVolume = @event.PetrolAmountInTransaction;
            nozzle.TotalPetrolAmount = @event.TotalDispatchedPetrolAmount;

           var report = factory.CreateReportForNozzleUsed(tank.Id, @event);

            if (petrolStation.Reports == null)
                petrolStation.Reports = new List<PetrolStationReport>();

            petrolStation.Reports.Add(report);

            tank.CurrentVolume -= @event.PetrolAmountInTransaction;

            await repository.UpdateAsync(petrolStation);

            await hub.Clients.All.SendAsync("StationUpdate", petrolStation.Id);

            logger.LogDebug($"Nozzle used event received handling ended.");
        }

        public async Task Handle(PetrolStolenByCustomer @event)
        {
            logger.LogDebug($"Petrol stolen by customer event received. Id: {@event.NozzleId}");

            var petrolStation = await repository.GetAsync(callContext.PetrolStationId.Value);

            petrolStation.Time = @event.TimeStamp;

            var tank = petrolStation.Tanks.SingleOrDefault(x => x.Nozzles.Any(z => z.Id == @event.NozzleId));

            tank.CurrentVolume -= @event.PetrolAmountInTransaction;

            var nozzle = tank.Nozzles.Single(z => z.Id == @event.NozzleId);
            nozzle.LastTransactionVolume = @event.PetrolAmountInTransaction;
            nozzle.TotalPetrolAmount = @event.TotalDispatchedPetrolAmount;
            nozzle.LastStolenPetrolAmount = @event.PetrolAmountInTransaction;
            nozzle.TotalStolenPetrolAmount = @event.TotalStolenPetrolAmount;

            var report = factory.CreateReportForPetrolStolenByCustomer(tank.Id, @event);

            if (petrolStation.Reports == null)
                petrolStation.Reports = new List<PetrolStationReport>();

            petrolStation.Reports.Add(report);

            await repository.UpdateAsync(petrolStation);

            await hub.Clients.All.SendAsync("StationUpdate", petrolStation.Id);

            logger.LogDebug($"Petrol stolen by customer event received handling ended.");
        }

        public async Task Handle(TankRefueled @event)
        {
            logger.LogDebug($"Tank refueled event received. Id: {@event.Id}");

            var petrolStation = await repository.GetAsync(callContext.PetrolStationId.Value);

            petrolStation.Time = @event.TimeStamp;

            var tank = petrolStation.Tanks.SingleOrDefault(x => x.Id == @event.Id);

            var report = factory.CreateReportForTankRefused(@event);

            if (petrolStation.Reports == null)
                petrolStation.Reports = new List<PetrolStationReport>();

            petrolStation.Reports.Add(report);

            tank.CurrentVolume = @event.CurrentPetrolVolume;

            await repository.UpdateAsync(petrolStation);

            await hub.Clients.All.SendAsync("StationUpdate", petrolStation.Id);

            logger.LogDebug($"Tank refueled event received handling ended.");
        }
    }
}
