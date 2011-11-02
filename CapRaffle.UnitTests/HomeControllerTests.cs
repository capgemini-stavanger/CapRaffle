using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CapRaffle.Controllers;
using System.Web.Mvc;

namespace CapRaffle.UnitTests
{
    class HomeControllerTests
    {

        private HomeController homeController;
        
        [SetUp]
        public void Setup()
        {
            homeController = new HomeController();
        }

        [Test]
        public void Can_About_Return_View()
        {
            var res = homeController.About();
            Assert.IsInstanceOf(typeof(ActionResult), res);
        }

        [Test]
        public void Can_Error_Return_View()
        {
            var res = homeController.Error();
            Assert.IsInstanceOf(typeof(ActionResult), res);
        }
    }
}
