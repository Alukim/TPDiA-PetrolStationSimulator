using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrolStation.Simulator.Entities
{
    public class Customer
    {
        public Customer(double order)
        {
            Order = order;
        }

        private double Order { get; }

        internal double GetOrder() => Order;

        internal bool PayForOrder()
        {
            Random random = new Random();
            bool hasCustomerPaid = false;
            var randomNumber = random.Next(1000);
            if (99 < randomNumber && randomNumber < 150)
                return hasCustomerPaid;
            else
            {
                hasCustomerPaid = true;
            }
            return hasCustomerPaid;
        }
    }
}
