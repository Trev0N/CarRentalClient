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
    public class LoginScreenTests
    {
        [TestMethod()]
        public void GetTokenTest()
        {
            String token = LoginScreen.GetToken("https://carrental-wsiz.herokuapp.com/api/login", "test", "test");
            Assert.AreEqual(token.Substring(17, 36).Length, "a7dc8209-24e3-4419-b47e-af4a884641b4".Length);


        
    }
    }
}