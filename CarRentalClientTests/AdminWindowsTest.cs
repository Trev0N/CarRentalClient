
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarRentalClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using System.Net.Http;
using Newtonsoft.Json;
using CarRentalClient.UtilClasses;

namespace CarRentalClient.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void GetRequestTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8080/car/")
                .Respond("application/json", "{\r\n    \"id\": 1,\r\n    \"registerName\": \"K1TEST\",\r\n    \"mark\": \"test1\",\r\n    \"model\": \"test2\",\r\n    \"engine\": 2000,\r\n    \"power\": 200,\r\n    \"garageId\": 1\r\n  },");
            var client = new HttpClient(mockHttp);
            var response = client.GetAsync("http://localhost:8080/car/").Result;

            Assert.AreEqual(response.Content.ReadAsStringAsync().Result, "{\r\n    \"id\": 1,\r\n    \"registerName\": \"K1TEST\",\r\n    \"mark\": \"test1\",\r\n    \"model\": \"test2\",\r\n    \"engine\": 2000,\r\n    \"power\": 200,\r\n    \"garageId\": 1\r\n  },");

        }

        [TestMethod()]
        public void PostRequestTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8080/car/create")
                .Respond(System.Net.HttpStatusCode.OK);
            var client = new HttpClient(mockHttp);
            HttpContent httpContent = null;
            var response = client.PostAsync("http://localhost:8080/car/create", httpContent).Result;

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
        }


        [TestMethod()]
        public void DeleteRequestTest()
        {
            long carId = 1;
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8080/car/"+carId)
                .Respond(System.Net.HttpStatusCode.NoContent);
            var client = new HttpClient(mockHttp);
            var response = client.DeleteAsync("http://localhost:8080/car/"+carId).Result;

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NoContent);
        }

        [TestMethod()]
        public void RequestToListCarTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8080/car/")
                .Respond("application/json", "[{\r\n    \"id\": 1,\r\n    \"registerName\": \"K1TEST\",\r\n    \"mark\": \"test1\",\r\n    \"model\": \"test2\",\r\n    \"engine\": 2000,\r\n    \"power\": 200,\r\n    \"garageId\": 1\r\n  }]");
            var client = new HttpClient(mockHttp);
            var response = client.GetAsync("http://localhost:8080/car/").Result;

            List<Car> cars = JsonConvert.DeserializeObject<List<Car>>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(cars.Count, 1);
            Assert.AreEqual(cars.ElementAt<Car>(0).ToString(), new Car(1, "K1TEST", "test1", "test2", 2000, 200, 1).ToString());

        }


        [TestMethod()]
        public void RequestToListGarageTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8080/garage/")
                .Respond("application/json","[\r\n  {\r\n    \"id\": 1,\r\n    \"name\": \"Fast Cars Rzeszow\",\r\n    \"address\": \"Rzeszow, ul. Lwowska 21\"\r\n  }\r\n]");
            var client = new HttpClient(mockHttp);
            var response = client.GetAsync("http://localhost:8080/garage/").Result;

            List<GarageList> garageList = JsonConvert.DeserializeObject<List<GarageList>>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(garageList.Count, 1);
            Assert.AreEqual(garageList.ElementAt<GarageList>(0).ToString(), new GarageList(1,"Fast Cars Rzeszow","Rzeszow, ul. Lwowska 21").ToString());
            Assert.AreNotEqual(garageList.ElementAt<GarageList>(0).ToString(), new GarageList(1, "Fast Cars Rzeszow", "Rzeszow, ul. Lwowska 1").ToString());
        }
        [TestMethod()]
        public void RequestToListCarDetailsTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8080/cardetail/")
                .Respond("application/json", "[\r\n  {\r\n    \"carId\": 1,\r\n    \"price\": 100,\r\n    \"statusEnum\": \"READY_TO_RENT\",\r\n    \"mileage\": 30\r\n  }\r\n]");
            var client = new HttpClient(mockHttp);
            var response = client.GetAsync("http://localhost:8080/cardetail/").Result;

            List<CarDetail> carDetailList = JsonConvert.DeserializeObject<List<CarDetail>>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(carDetailList.Count, 1);
            Assert.AreEqual(new CarDetail(1,Status.READY_TO_RENT,100,30).ToString(),carDetailList.ElementAt<CarDetail>(0).ToString());
        }


    }
}