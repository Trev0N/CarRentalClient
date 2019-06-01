using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalClient.UtilClasses
{/// <summary>
/// Klasa pozwalajaca zserializowac dane dla CarDetail
/// </summary>
    public class CarDetail
    {
        public long CarID { get; }

        public decimal price { get; }

        public Status status { get; set; }

        public long mileage { get; }

        public CarDetail(long carId, Status statusEnum, decimal price, long mileage)
        {
            this.CarID = carId;
            this.price = price;
            this.mileage = mileage;
            this.status = statusEnum;
        }

        public override string ToString()
        {
            return CarID.ToString();
        }
    }

    public enum Status
    {
        READY_TO_RENT,
        NEED_ATTENTION,
        SERVICE_PLEASE,
        ON_SERVICE,
        RENTED,
        NO_FUEL
    }

}
