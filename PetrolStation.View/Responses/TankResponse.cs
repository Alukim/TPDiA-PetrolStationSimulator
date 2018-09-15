using System;
using System.Collections.Generic;

namespace PetrolStation.View.Responses
{
    public class TankResponse
    {
        public TankResponse(Guid id, double maximumVolume, double currentVolume, decimal petrolTemperature, decimal tankHigh, IReadOnlyCollection<NozzleResponse> nozzleResponses)
        {
            Id = id;
            MaximumVolume = maximumVolume;
            CurrentVolume = currentVolume;
            PetrolTemperature = petrolTemperature;
            TankHigh = tankHigh;
            NozzleResponses = nozzleResponses;
        }

        public Guid Id { get; set; }
        public double MaximumVolume { get; set; }
        public double CurrentVolume { get; set; }
        public decimal PetrolTemperature { get; set; }
        public decimal TankHigh { get; set; }
        public IReadOnlyCollection<NozzleResponse> NozzleResponses { get; }
    }
}
