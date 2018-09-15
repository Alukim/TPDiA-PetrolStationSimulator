using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NodaTime;
using PetrolStation.Infrastructure;
using PetrolStation.Simulator.Contracts.Events;
using PetrolStation.Simulator.Exceptions;

namespace PetrolStation.Simulator.Entities
{
    public class PetrolStation : IEntity
    {
        public PetrolStation(Guid id, string name, List<Tank> tanks, List<Nozzle> nozzles, DateTime currentDateTime)
        {
            Id = id;
            Name = name;
            Tanks = tanks;
            Nozzles = nozzles;
            CurrentDateTime = currentDateTime;
            Random = new Random();
            EventsToSend = new Queue<IEvent>();
        }

        public Guid Id { get; }
        public string Name { get; }
        public List<Tank> Tanks { get; }
        public List<Nozzle> Nozzles { get; }
        public DateTime CurrentDateTime { get; private set; }
        private Random Random { get; }

        [JsonIgnore]
        public Queue<IEvent> EventsToSend { get; } 

        public void Created()
        {
            var @event = new PetrolStationCreated(
                id: Id,
                name: Name,
                tanks: Tanks.Select(x => x.ToContract()).ToList(),
                nozzles: Nozzles.Select(x => x.ToContract()).ToList());
            EventsToSend.Enqueue(@event);
        }

        public void SimulateActivity()
        {
            SimulateArrivingCustomers();
            HandleQueuesAtNozzles();
        }

        public List<RefuelOrder> GetOrders() => Tanks.Select(x => x.GetOrder()).Where(x => x != null).ToList();

        private void HandleQueuesAtNozzles()
        {
            foreach (var nozzle in Nozzles)
            {
                try
                {
                    var @event = nozzle.HandleQueue(CurrentDateTime, Tanks.SingleOrDefault(x => x.Id == nozzle.TankId));
                    if (@event != null)
                        EventsToSend.Enqueue(@event);
                }
                catch (NotEnoughFuel e)
                {
                    var customer = e.UnhandledCustomer;
                    var newNozzle = FindNozzleForCustomer(nozzle);
                    newNozzle.AddCustomerToQueue(customer);
                }
            }
        }

        internal void RefuelTanks(List<Supply> supplies)
        {
            foreach (var tank in Tanks)
            {
                var supply = supplies.SingleOrDefault(x => x.TankId == tank.Id);
                if (supply != null)
                {
                    var @event = tank.Refuel(supply.PetrolAmount, supply.PetrolLoss, CurrentDateTime);
                    EventsToSend.Enqueue(@event);
                }
            }
        }

        private void SimulateArrivingCustomers()
        {
            List<Customer> customers = GenerateCustomers();
            foreach(var customer in customers)
            {
                var nozzle = FindNozzleForCustomer(null);
                nozzle.AddCustomerToQueue(customer);
            }
        }

        private Nozzle FindNozzleForCustomer(Nozzle lastNozzle)
        {
            Nozzle bestNozzle = null;
            Nozzle lowestNozzle = null;
            do
            {
                lowestNozzle = bestNozzle;
                bestNozzle = Nozzles.ElementAt(Random.Next(Nozzles.Count));
                if (bestNozzle.GetCustomerQueueSize() == 0)
                    break;

                if(lastNozzle != null && bestNozzle.Id == lastNozzle.Id)
                    bestNozzle = Nozzles.ElementAt(Random.Next(Nozzles.Count));

            } while (bestNozzle.GetCustomerQueueSize() != 0 && bestNozzle.GetCustomerQueueSize() > (lowestNozzle?.GetCustomerQueueSize() ?? 0));

            return bestNozzle;

        }

        private List<Customer> GenerateCustomers()
        {
            var customers = new List<Customer>();
            int customersCount = GenerateCustomersCount();

            for (int i = 0; i < customersCount; i++)
            {
                customers.Add(new Customer(GenerateCustomersOrder()));
            }
            return customers;
        }

        private double GenerateCustomersOrder()
        {
            return Random.Next(2, 100);
        }

        private int GenerateCustomersCount()
        {
            if (CurrentDateTime.TimeOfDay < TimeSpan.FromHours(6))
                return Random.Next(4);
            else if (CurrentDateTime.TimeOfDay < TimeSpan.FromHours(9))
                return Random.Next(8);
            else if (CurrentDateTime.TimeOfDay < TimeSpan.FromHours(12))
                return Random.Next(6);
            else if (CurrentDateTime.TimeOfDay < TimeSpan.FromHours(16))
                return Random.Next(10);
            else if (CurrentDateTime.TimeOfDay < TimeSpan.FromHours(20))
                return Random.Next(6);
            else
                return Random.Next(5);
        }

        public void ChangeCurrentBusinessDate(double minuteInterval)
        {
            CurrentDateTime = CurrentDateTime.AddMinutes(minuteInterval);
        }
    }
}
