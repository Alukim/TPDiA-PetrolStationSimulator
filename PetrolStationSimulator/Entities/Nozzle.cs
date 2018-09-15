using Newtonsoft.Json;
using NodaTime;
using PetrolStation.Infrastructure;
using PetrolStation.Simulator.Contracts.Entities;
using PetrolStation.Simulator.Contracts.Events;
using PetrolStation.Simulator.Exceptions;
using System;
using System.Collections.Generic;

namespace PetrolStation.Simulator.Entities
{
    public class Nozzle
    {
        public Nozzle(Guid id, Guid tankId, double lastTransactionVolume, double totalStolenPetrolAmount,
            double lastStolenPetrolAmount, double petrolAmountInCurrentTransaction)
        {
            Id = id;
            TankId = tankId;
            LastTransactionVolume = lastTransactionVolume;
            TotalStolenPetrolAmount = totalStolenPetrolAmount;
            LastStolenPetrolAmount = lastStolenPetrolAmount;
            PetrolAmountInCurrentTransaction = petrolAmountInCurrentTransaction;
        }

        public Guid Id { get; }
        public Guid TankId { get; }
        public double LastTransactionVolume { get; private set; }
        public double TotalStolenPetrolAmount { get; private set; }
        public double LastStolenPetrolAmount { get; private set; }
        public double PetrolAmountInCurrentTransaction { get; private set; }
        public double TotalPetrolAmount { get; set; }

        [JsonIgnore]
        private Queue<Customer> Customers { get; } = new Queue<Customer>();
        private bool IsCurrentlyInUse = false;
        private Customer CurrentClient { get; set; }

        public int GetCustomerQueueSize() => Customers.Count;
        public bool IsQueueEmpty() => Customers.Count == 0;

        internal IEvent HandleQueue(DateTime currentDateTime, Tank tank)
        {
            if (IsCurrentlyInUse)
                return FinishUsingNozzle(currentDateTime);

            if (IsQueueEmpty())
                return null;

            IsCurrentlyInUse = true;
            CurrentClient = Customers.Dequeue();
            StartUsingNozzle(CurrentClient.GetOrder(), currentDateTime, tank);
            return null;
        }

        private void StartUsingNozzle(double petrolAmount, DateTime currentDateTime, Tank tank)
        {
            try
            {
                tank.ReservePetrol(petrolAmount, currentDateTime);
            }
            catch (NotEnoughFuel e)
            {
                IsCurrentlyInUse = false;
                e.SetUnhandlerCustomer(CurrentClient);
                throw e;
            }
            PetrolAmountInCurrentTransaction = petrolAmount;
            LastStolenPetrolAmount = 0;
        }

        private IEvent FinishUsingNozzle(DateTime currentDateTime)
        {
            TotalPetrolAmount += PetrolAmountInCurrentTransaction;
            IsCurrentlyInUse = false;
            IEvent eventToPublish;
            if (CurrentClient.PayForOrder())
            {
                TotalStolenPetrolAmount += 0;
                LastStolenPetrolAmount = 0;
                eventToPublish = new NozzleUsed(Id, currentDateTime, PetrolAmountInCurrentTransaction, TotalPetrolAmount);
                PetrolAmountInCurrentTransaction = 0;
            }
            else
            {
                TotalStolenPetrolAmount += PetrolAmountInCurrentTransaction;
                LastStolenPetrolAmount = PetrolAmountInCurrentTransaction;
                eventToPublish = new PetrolStolenByCustomer(Id, currentDateTime, LastStolenPetrolAmount, TotalStolenPetrolAmount, TotalPetrolAmount);
                PetrolAmountInCurrentTransaction = 0;
            }

            return eventToPublish;
        }

        internal void AddCustomerToQueue(Customer customer)
        {
            Customers.Enqueue(customer);
        }

        public NozzleContract ToContract()
            => new NozzleContract(
                id: Id,
                tankId: TankId,
                lastTransactionVolume: LastTransactionVolume,
                totalStolenPetrolAmount: TotalStolenPetrolAmount,
                lastStolenPetrolAmount: LastStolenPetrolAmount,
                petrolAmountInCurrentTransaction: PetrolAmountInCurrentTransaction);
    }

}