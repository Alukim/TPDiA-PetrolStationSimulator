using PetrolStation.Infrastructure;
using System;

namespace PetrolStation.Simulator.Contracts.Events
{
    public class PetrolStolenByCustomer : IEvent
    {
        public PetrolStolenByCustomer(Guid nozzleId, DateTime timeStamp, double petrolAmountInTransaction, double totalStolenPetrolAmount, double totalDispatchedPetrolAmount)
        {
            NozzleId = nozzleId;
            TimeStamp = timeStamp;
            PetrolAmountInTransaction = petrolAmountInTransaction;
            TotalStolenPetrolAmount = totalStolenPetrolAmount;
            TotalDispatchedPetrolAmount = totalDispatchedPetrolAmount;
        }

        public Guid NozzleId { get; }

        public DateTime TimeStamp { get; }

        public double PetrolAmountInTransaction { get; }

        public double TotalStolenPetrolAmount { get; }

        public double TotalDispatchedPetrolAmount { get; }
    }
}
