using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using CapRaffle.Controllers;
using CapRaffle.Models;
using Moq;
using System.Web.Routing;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    class NavControllerTests
    {
        [Test]
        public void Can_get_correct_selected_menu()
        {
            var mockHttpContext = new Mock<ControllerContext>();

            mockHttpContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("arne.aase@capgemini.com");
            mockHttpContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            var controller =  new NavController();
            controller.ControllerContext = new ControllerContext(mockHttpContext.Object.HttpContext, new RouteData(), controller);
            var result = (PartialViewResult)controller.Menu("Event", "Index");
                
            var viewModel = (IEnumerable<MenuViewModel>)result.Model;

            Assert.IsInstanceOf(typeof(IEnumerable<MenuViewModel>), result.Model);
            Assert.AreEqual(true, viewModel.Where(x => x.Controller.Equals("event") && x.Action.Equals("index")).FirstOrDefault().isSelected);
        }
    }
}
