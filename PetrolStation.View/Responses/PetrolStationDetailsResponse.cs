using System;
using System.Collections.Generic;

namespace PetrolStation.View.Responses
{
    public class PetrolStationDetailsResponse
    {
        public PetrolStationDetailsResponse(Guid id, string name, DateTime time, IReadOnlyCollection<TankResponse> tanks, IReadOnlyCollection<PetrolStationRaportResponse> raports)
        {
            Id = id;
            Name = name;
            Time = time;
            Tanks = tanks;
            Raports = raports;
        }

        public Guid Id { get; }

        public string Name { get; }

        public DateTime Time { get; }

        public IReadOnlyCollection<TankResponse> Tanks { get; }
        public IReadOnlyCollection<PetrolStationRaportResponse> Raports { get; }
    }
}
