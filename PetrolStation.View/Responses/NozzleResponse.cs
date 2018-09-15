using System;

namespace PetrolStation.View.Responses
{
    public class NozzleResponse
    {
        public NozzleResponse(Guid id, double lastTransactionVolume, double totalStolenPetrolAmount, double lastStolenPetrolAmount, double totalPetrolAmount)
        {
            Id = id;
            LastTransactionVolume = lastTransactionVolume;
            TotalStolenPetrolAmount = totalStolenPetrolAmount;
            LastStolenPetrolAmount = lastStolenPetrolAmount;
            TotalPetrolAmount = totalPetrolAmount;
        }

        public Guid Id { get; set; }
        public double LastTransactionVolume { get; set; }
        public double TotalStolenPetrolAmount { get; set; }
        public double LastStolenPetrolAmount { get; set; }
        public double TotalPetrolAmount { get; set; }
    }
}
