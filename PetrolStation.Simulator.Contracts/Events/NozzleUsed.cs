using PetrolStation.Infrastructure;
using System;

namespace PetrolStation.Simulator.Contracts.Events
{
    public class NozzleUsed : IEvent
    {
        public NozzleUsed(Guid id, DateTime timeStamp, double petrolAmountInTransaction, double totalDispatchedPetrolAmount)
        {
            Id = id;
            TimeStamp = timeStamp;
            PetrolAmountInTransaction = petrolAmountInTransaction;
            TotalDispatchedPetrolAmount = totalDispatchedPetrolAmount;
        }

        public Guid Id { get; }

        public DateTime TimeStamp { get; }

        public double PetrolAmountInTransaction { get; }

        public double TotalDispatchedPetrolAmount { get; }
    }
}
