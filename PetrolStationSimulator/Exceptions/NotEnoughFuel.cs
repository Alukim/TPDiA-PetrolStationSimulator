using PetrolStation.Simulator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrolStation.Simulator.Exceptions
{
    public class NotEnoughFuel : Exception
    {
        public Customer UnhandledCustomer { get; private set; }
        public void SetUnhandlerCustomer(Customer customer)
        {
            UnhandledCustomer = customer;
        }
    }
}
