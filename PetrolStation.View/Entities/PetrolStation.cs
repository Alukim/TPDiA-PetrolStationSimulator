using PetrolStation.Infrastructure;
using System;
using System.Collections.Generic;

namespace PetrolStation.View.Entities
{
    public class PetrolStation : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime Time { get; set; }

        public List<Tank> Tanks { get; set; }

        public List<PetrolStationReport> Reports { get; set; }
    }
}
