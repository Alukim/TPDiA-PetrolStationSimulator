using System;

namespace PetrolStation.Simulator.Contracts.Entities
{
    public class NozzleContract
    {
        public NozzleContract(Guid id, Guid tankId, double lastTransactionVolume, double totalStolenPetrolAmount, double lastStolenPetrolAmount, double petrolAmountInCurrentTransaction)
        {
            Id = id;
            TankId = tankId;
            LastTransactionVolume = lastTransactionVolume;
            TotalStolenPetrolAmount = totalStolenPetrolAmount;
            LastStolenPetrolAmount = lastStolenPetrolAmount;
            PetrolAmountInCurrentTransaction = petrolAmountInCurrentTransaction;
        }

        public Guid Id { get; }
        public Guid TankId { get; }
        /// <summary>
        /// To to w sumie nie wiem po co tu jest XD Nie zwróciłem na to uwagi pisząc eventy, pewnie dlatego tego nie wykorzystałem. Ważne jest w sumie tylko Id i TankId
        /// </summary>
        public double LastTransactionVolume { get; }
        public double TotalStolenPetrolAmount { get; }
        public double LastStolenPetrolAmount { get; }
        public double PetrolAmountInCurrentTransaction { get; }
    }
}
