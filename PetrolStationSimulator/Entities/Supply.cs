using System;

namespace PetrolStation.Simulator.Entities
{
    public class Supply
    {
        public Supply(Guid tankId, double petrolAmount)
        {
            TankId = tankId;
            PetrolAmount = petrolAmount;
            GenerateLoss();
        }

        public Guid TankId { get; }
        public double PetrolAmount { get; private set;}
        public double PetrolLoss { get; private set; }

        void GenerateLoss()
        {
            Random random = new Random();
            if (random.Next(100) == 99)
            {
                PetrolLoss = 0.01d * PetrolAmount;
                PetrolAmount = PetrolAmount - PetrolLoss;
            }
        }

    }
}
