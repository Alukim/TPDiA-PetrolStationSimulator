using NodaTime;
using PetrolStation.Infrastructure;
using PetrolStation.Simulator.Exceptions;
using PetrolStation.Simulator.Contracts.Entities;
using System;
using PetrolStation.Simulator.Contracts.Events;
using Newtonsoft.Json;

namespace PetrolStation.Simulator.Entities
{
    public class Tank
    {
        private const double minVolume = 10000d;

        public Tank(Guid id, double maximumVolume, double currentVolume, decimal petrolTemperature, decimal tankHigh, RefuelOrder refuelOrder)
        {
            Id = id;
            MaximumVolume = maximumVolume;
            CurrentVolume = currentVolume;
            PetrolTemperature = petrolTemperature;
            TankHigh = tankHigh;
            RefuelOrder = refuelOrder;
        }

        public Guid Id { get; }
        public double MaximumVolume { get; }
        public double CurrentVolume { get; set; }
        public decimal PetrolTemperature { get; set; }
        public decimal TankHigh { get; set; }

        public RefuelOrder RefuelOrder { get; private set; }

        internal void ReservePetrol(double petrolAmount, DateTime currentDateTime)
        {
            if (CurrentVolume < petrolAmount)
                throw new NotEnoughFuel();

            CurrentVolume -= petrolAmount;
            OrderSupplyIfNeeded(currentDateTime);
        }

        internal RefuelOrder GetOrder() => RefuelOrder;

        private void OrderSupplyIfNeeded(DateTime DateTime)
        {
            if (CurrentVolume <= minVolume)
                if (RefuelOrder == null)
                    RefuelOrder = new RefuelOrder(Id, DateTime, MaximumVolume - CurrentVolume);
        }

        internal IEvent Refuel(double petrolAmount, double petrolLoss, DateTime currentDateTime)
        {
            double leakage = CalculateLeakage(petrolAmount);
            CurrentVolume += (petrolAmount - leakage);

            RefuelOrder = null;
            return new TankRefueled(Id, currentDateTime, CurrentVolume, petrolAmount - leakage, leakage, petrolLoss);
        }

        private double CalculateLeakage(double petrolAmount)
        {
            var random = new Random();
            if (random.Next(100) > 98)
                return petrolAmount * random.NextDouble();

            return 0d;
        }

        public TankContract ToContract()
            => new TankContract(
                id: Id,
                maximumVolume: MaximumVolume,
                currentVolume: CurrentVolume,
                petrolTemperature: PetrolTemperature,
                tankHigh: TankHigh);
    }
}