using PetrolStation.Simulator.Contracts.Events;
using PetrolStation.View.Entities;
using System;

namespace PetrolStation.View.Factories
{
    public class PetrolStationReportFactory
    {
        public PetrolStationReport CreateReportForNozzleUsed(Guid tankId, NozzleUsed @event)
        {
            return new PetrolStationReport()
            {
                TankId = tankId,
                NozzleId = @event.Id,
                TimeStamp = @event.TimeStamp,
                Report = $"Nozzle was used. Petrol amount in transaction: {@event.PetrolAmountInTransaction}. Total dispatched petrol amount by nozzle: {@event.TotalDispatchedPetrolAmount}.",
                Status = ReportStatus.NozzleUsed
            };
        }

        public PetrolStationReport CreateReportForPetrolStolenByCustomer(Guid tankId, PetrolStolenByCustomer @event)
        {
            return new PetrolStationReport()
            {
                TankId = tankId,
                NozzleId = @event.NozzleId,
                TimeStamp = @event.TimeStamp,
                Report = $"Petrol stolen by customer. Petrol amount in transaction: {@event.PetrolAmountInTransaction}. Total stolen petrol amount: {@event.TotalStolenPetrolAmount}. Total dispatched petrol amount: {@event.TotalDispatchedPetrolAmount}.",
                Status = ReportStatus.PetrolStolenByCustomer
            };
        }

        public PetrolStationReport CreateReportForTankRefused(TankRefueled @event)
        {
            return new PetrolStationReport()
            {
                TankId = @event.Id,
                NozzleId = null,
                TimeStamp = @event.TimeStamp,
                Report = $"Tank refueled. Current petrol volume: {@event.CurrentPetrolVolume}. Arrived petrol amount: {@event.ArrivedPetrolAmount}. Leaked petrol amount: {@event.LeakedPetrolAmount}. Stolen petrol amount {@event.StolenPetrolAmount}",
                Status = ReportStatus.TankRefueled
            };
        }
    }
}
