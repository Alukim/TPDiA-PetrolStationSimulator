using Newtonsoft.Json;
using System;

namespace PetrolStation.View.Responses
{
    public class PetrolStationRaportResponse
    {
        public PetrolStationRaportResponse(Guid tankId, Guid? nozzleId, DateTime timeStamp, string report, string status)
        {
            TankId = tankId;
            NozzleId = nozzleId;
            TimeStamp = timeStamp;
            Report = report;
            Status = status;
        }

        public Guid TankId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public Guid? NozzleId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Report { get; set; }

        public string Status { get; set; }
    }
}
