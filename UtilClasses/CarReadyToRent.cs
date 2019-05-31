using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalClient
{/// <summary>
/// Klasa pozwalajaca zserializowac dane dla CarReadyToRent
/// </summary>
    class CarReadyToRent
    {
        public long ID { get; }

        public String RegisterName { get; }

        public String Mark { get; }

        public String Model { get; }

        public int Engine { get; }

        public int Power { get; }

        public long GarageID { get; }

        public decimal Price { get; }

        public CarReadyToRent(long iD, string registerName, string mark, string model, int engine, int power, long garageID, decimal price)
        {
            ID = iD;
            RegisterName = registerName;
            Mark = mark;
            Model = model;
            Engine = engine;
            Power = power;
            GarageID = garageID;
            Price = price;
        }
    }
}
