using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalClient
{
    class GarageList
    {

        public String Name { get; }

        public String Address { get; }

        public GarageList(string name, string address)
        {
            Name = name;
            Address = address;
        }
    }
}
