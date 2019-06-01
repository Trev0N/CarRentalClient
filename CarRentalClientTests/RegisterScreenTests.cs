using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarRentalClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.IO;

namespace CarRentalClient.Tests
{
    [TestClass()]
    public class RegisterScreenTests
    {
        [TestMethod()]
        public void PostRegisterTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8080/user/sign-in")
                .Respond(System.Net.HttpStatusCode.Created);
            var client = new HttpClient(mockHttp);

            HttpContent httpContent = null;
            var response = client.PostAsync("http://localhost:8080/user/sign-in", httpContent).Result;

            Assert.AreEqual(System.Net.HttpStatusCode.Created.ToString(), response.StatusCode.ToString());
        }
    }
}