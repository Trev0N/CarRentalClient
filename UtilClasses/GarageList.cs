using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalClient
{
    class GarageList
    {

        public long ID;

        public String Name { get; }

        public String Address { get; }

        public GarageList(long id,string name, string address)
        {
            ID = id;
            Name = name;
            Address = address;
        }




        public override string ToString()
        {
            return this.Name +" * "+ this.Address;
        }
    }
}
