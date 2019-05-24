using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalClient.UtilClasses
{
    class CarDetail
    {
        public long CarID { get; }

        public decimal price { get; }

        public Status status { get; }

        public long mileage { get; }

        public CarDetail(long carId, Status status, decimal price, long mileage)
        {
            this.CarID = carId;
            this.price = price;
            this.mileage = mileage;
            this.status = status;
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
