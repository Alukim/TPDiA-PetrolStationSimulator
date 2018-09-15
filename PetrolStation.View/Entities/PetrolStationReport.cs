using Newtonsoft.Json;
using System;

namespace PetrolStation.View.Entities
{
    public enum ReportStatus
    {
        NozzleUsed = 1,
        PetrolStolenByCustomer = 2,
        TankRefueled = 3
    }

    public class PetrolStationReport
    {
        public Guid TankId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public Guid? NozzleId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Report { get; set; }

        public ReportStatus Status { get; set; }
    }
}
