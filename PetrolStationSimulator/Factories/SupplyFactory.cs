using NodaTime;
using PetrolStation.Simulator.Entities;
using System;
using System.Collections.Generic;

namespace PetrolStation.Simulator.Factories
{
    public static class SupplyFactory
    {
        public static List<Supply> CreateFor(List<RefuelOrder> refuelOrders, DateTime currentDateTime)
        {
            var result = new List<Supply>();
            foreach (var refuelOrder in refuelOrders)
            {
                if ((currentDateTime - refuelOrder.TimeStamp).Days > 1)
                    result.Add(new Supply(refuelOrder.TankId, refuelOrder.ExpectedPetrolAmount));
            }

            return result;
        }
    }
}
