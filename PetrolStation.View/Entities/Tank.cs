using System;
using System.Collections.Generic;

namespace PetrolStation.View.Entities
{
    public class Tank
    {
        public Guid Id { get; set; }
        public double MaximumVolume { get; set; }
        public double CurrentVolume { get; set; }
        public decimal PetrolTemperature { get; set; }
        public decimal TankHigh { get; set; }

        public List<Nozzle> Nozzles { get; set; }
    }
}