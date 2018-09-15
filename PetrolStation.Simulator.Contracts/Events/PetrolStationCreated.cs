using PetrolStation.Infrastructure;
using PetrolStation.Simulator.Contracts.Entities;
using System;
using System.Collections.Generic;

namespace PetrolStation.Simulator.Contracts.Events
{
    public class PetrolStationCreated : IEvent
    {
        public PetrolStationCreated(Guid id, string name, IReadOnlyCollection<TankContract> tanks, IReadOnlyCollection<NozzleContract> nozzles)
        {
            Id = id;
            Name = name;
            Tanks = tanks;
            Nozzles = nozzles;
        }

        public Guid Id { get; }

        public string Name { get; }

        public IReadOnlyCollection<TankContract> Tanks { get; }
        public IReadOnlyCollection<NozzleContract> Nozzles { get; }
    }
}
