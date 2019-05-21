using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalClient
{
    class Car
    {
        public long ID { get; set; }

        public String RegisterName { get; set; }

        public String Mark { get; set; }

        public String Model { get; set; }

        public int Engine { get; set; }

        public int Power { get; set; }

        public long GarageID { get; set; }

        public Car(long iD, string registerName, string mark, string model, int engine, int power, long garageID)
        {
            ID = iD;
            RegisterName = registerName;
            Mark = mark;
            Model = model;
            Engine = engine;
            Power = power;
            GarageID = garageID;
        }

        public override string ToString()
        {
            return "ID=" + ID + ", RegisterName=" + RegisterName + ", Mark=" + Mark + ", Model=" + Model + ", Engine=" + Engine + ", Power=" + Power + ", GarageID=" + GarageID;
        }
    }
}
