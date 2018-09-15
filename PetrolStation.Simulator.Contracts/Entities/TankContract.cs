using System;

namespace PetrolStation.Simulator.Contracts.Entities
{
    public class TankContract
    {
        public TankContract(Guid id, double maximumVolume, double currentVolume, decimal petrolTemperature, decimal tankHigh)
        {
            Id = id;
            MaximumVolume = maximumVolume;
            CurrentVolume = currentVolume;
            PetrolTemperature = petrolTemperature;
            TankHigh = tankHigh;
        }

        public Guid Id { get; }
        /// <summary>
        /// Maksymalna ilość paliwa w zbiorniku
        /// </summary>
        public double MaximumVolume { get; }
        /// <summary>
        /// Aktualna ilość paliwa w zbiorniku. Dla `PetrolStationCreated` powinna być równa maksymalnej ilości
        /// </summary>
        public double CurrentVolume { get; }
        /// <summary>
        /// Temperatura paliwa. Nic z tym nie robimy, ale było w poprzednej wersji, więc noo.
        /// </summary>
        public decimal PetrolTemperature { get; }
        /// <summary>
        /// Podobnie jak wyżej.
        /// </summary>
        public decimal TankHigh { get; }
    }
}
