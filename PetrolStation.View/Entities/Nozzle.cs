using System;

namespace PetrolStation.View.Entities
{
    public class Nozzle
    {
        public Guid Id { get; set; }
        public double LastTransactionVolume { get; set; }
        public double TotalStolenPetrolAmount { get; set; }
        public double LastStolenPetrolAmount { get; set; }
        public double TotalPetrolAmount { get; set; }
    }
}