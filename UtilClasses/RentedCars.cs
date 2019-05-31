using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalClient
{/// <summary>
/// Klasa pozwalajaca zserializowac dane dla RentedCars
/// </summary>
    class RentedCars
    {
        public long IDRent;

        public DateTime RentStartDate { get; }

        public DateTime RentEndDate { get; }

        public long IDCar;

        public String RegisterName { get; }

        public String Mark { get; }

        public String Model { get; }

        public int Engine { get; }

        public int Power { get; }

        public String GarageName { get; }

        public String GarageAddress { get; }

        public RentedCars(long iDRent, DateTime rentStartDate, DateTime rentEndDate, long iDCar, string registerName, string mark, string model, int engine, int power, string garageName, string garageAddress)
        {
            IDRent = iDRent;
            RentStartDate = rentStartDate;
            RentEndDate = rentEndDate;
            IDCar = iDCar;
            RegisterName = registerName;
            Mark = mark;
            Model = model;
            Engine = engine;
            Power = power;
            GarageName = garageName;
            GarageAddress = garageAddress;
        }
        
       
    }
}
