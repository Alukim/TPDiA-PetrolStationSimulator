using Newtonsoft.Json;
using System;

namespace PetrolStation.Infrastructure
{
    public class CallContext
    {
        [JsonConstructor]
        public CallContext(Guid? petrolStationId, string petrolStationName)
        {
            PetrolStationId = petrolStationId;
            PetrolStationName = petrolStationName;
        }

        public Guid? PetrolStationId { get; set; }

        public string PetrolStationName { get; set; }

        public static CallContext CreateEmpty()
        {
            return new CallContext(
                petrolStationId: null,
                petrolStationName: null);
        }

        public void InitializeCallContextFrom(CallContext ctx)
        {
            PetrolStationId = ctx.PetrolStationId;
            PetrolStationName = ctx.PetrolStationName;
        }
    }
}
