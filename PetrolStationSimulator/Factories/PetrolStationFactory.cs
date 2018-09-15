using Microsoft.Extensions.Options;
using NodaTime;
using PetrolStation.Infrastructure;
using PetrolStation.Simulator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetrolStation.Simulator.Factories
{
    public class PetrolStationFactory
    {
        const int TANKS_COUNT = 4;
        const double MAX_TANK_VOLUME = 20000d;
        const int TANK_HIGH = 1500;
        const int NOZZLES_COUNT_PER_TANK = 4;

        private readonly GlobalSettings globalSettings;
        public PetrolStationFactory(IOptions<GlobalSettings> options)
        {
            globalSettings = options.Value;
        }

        public Entities.PetrolStation Create()
        {
            var tanks = Enumerable.Range(0, TANKS_COUNT).Select(x => new Tank(Guid.NewGuid(), MAX_TANK_VOLUME, MAX_TANK_VOLUME, 15, TANK_HIGH, null)).ToList();
            var nozzles = new List<Nozzle>();
            foreach (var tank in tanks)
            {
                nozzles.AddRange(Enumerable.Range(0, NOZZLES_COUNT_PER_TANK).Select(x => new Nozzle(Guid.NewGuid(), tank.Id, 0d, 0d, 0d, 0d)).ToList());
            }

            var petrolStation = new Entities.PetrolStation(globalSettings.ServiceId, globalSettings.ServiceName, tanks.ToList(), nozzles, DateTime.UtcNow);
            petrolStation.Created();
            return petrolStation;
        }
    }
}
