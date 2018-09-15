using System;

namespace PetrolStation.View.Responses
{
    public class PetrolStationResponse
    {
        public PetrolStationResponse(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
