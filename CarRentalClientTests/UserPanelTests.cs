using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarRentalClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using System.Net.Http;

namespace CarRentalClient.Tests
{
    [TestClass()]
    public class UserPanelTests
    {
        [TestMethod()]
        public void GetRequestTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8080/car/readytorent")
                .Respond("application/json", "[\r\n  {\r\n    \"id\": 1,\r\n    \"registerName\": \"RZ 20202\",\r\n    \"mark\": \"Skoda\",\r\n    \"model\": \"Superb\",\r\n    \"engine\": 2000,\r\n    \"power\": 170,\r\n    \"garageId\": 1,\r\n    \"price\": 100\r\n  }\r\n]");
            var client = new HttpClient(mockHttp);
            var response = client.GetAsync("http://localhost:8080/car/readytorent").Result;

            Assert.AreEqual(response.Content.ReadAsStringAsync().Result, "[\r\n  {\r\n    \"id\": 1,\r\n    \"registerName\": \"RZ 20202\",\r\n    \"mark\": \"Skoda\",\r\n    \"model\": \"Superb\",\r\n    \"engine\": 2000,\r\n    \"power\": 170,\r\n    \"garageId\": 1,\r\n    \"price\": 100\r\n  }\r\n]");

        }

        [TestMethod()]
        public void PostRequestTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8080/rent/create")
                .Respond(System.Net.HttpStatusCode.Created);
            var client = new HttpClient(mockHttp);
            HttpContent httpContent = null;
            var response = client.PostAsync("http://localhost:8080/rent/create", httpContent).Result;

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.Created);
        }

    }
}