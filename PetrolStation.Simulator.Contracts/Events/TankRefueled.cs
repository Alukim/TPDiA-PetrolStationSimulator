using PetrolStation.Infrastructure;
using System;

namespace PetrolStation.Simulator.Contracts.Events
{
    public class TankRefueled : IEvent
    {
        public TankRefueled(Guid id, DateTime timeStamp, double currentPetrolVolume, double arrivedPetrolAmount, double leakedPetrolAmount, double stolenPetrolAmount)
        {
            Id = id;
            TimeStamp = timeStamp;
            CurrentPetrolVolume = currentPetrolVolume;
            ArrivedPetrolAmount = arrivedPetrolAmount;
            LeakedPetrolAmount = leakedPetrolAmount;
            StolenPetrolAmount = stolenPetrolAmount;
        }

        public Guid Id { get; }

        public DateTime TimeStamp { get; }

        public double CurrentPetrolVolume { get; }

        public double ArrivedPetrolAmount { get; }

        public double LeakedPetrolAmount { get; }

        public double StolenPetrolAmount { get; }
    }
}
