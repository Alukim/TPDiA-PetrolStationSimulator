using NodaTime;
using System;

namespace PetrolStation.Simulator.Entities
{
    public class RefuelOrder
    {
        public RefuelOrder(Guid tankId, DateTime timeStamp, double expectedPetrolAmount)
        {
            TankId = tankId;
            TimeStamp = timeStamp;
            ExpectedPetrolAmount = expectedPetrolAmount;
        }

        public Guid TankId { get; }
        public DateTime TimeStamp { get; }
        public double ExpectedPetrolAmount { get; }
    }
}
