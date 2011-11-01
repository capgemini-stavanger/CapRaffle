using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CapRaffle.ActionFilterAttributes;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Rhino.Mocks;
using Moq;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    class AttributeTests : Shared
    {
        [Test]
        public void Can_get_correct_viewbag_from_set_selected_menu_attribute()
        {
            var attribute = new SetSelectedMenuAttribute();
            var routedata = new RouteData();
            routedata.Values.Add("controller", "Event");
            routedata.Values.Add("action", "Index");
            var httpcontext = new Mock<HttpContextBase>(MockBehavior.Loose);
            var context = new ControllerContext(httpcontext.Object, routedata, eventController);

            var filtercontext = new ActionExecutingContext(
                context, 
                Rhino.Mocks.MockRepository.GenerateStub<ActionDescriptor>(), 
                new Dictionary<string, object>());
            
            attribute.OnActionExecuting(filtercontext);

            Assert.AreEqual("Event", eventController.ViewBag.MenuController);
            Assert.AreEqual("Index", eventController.ViewBag.MenuAction);
        }
    }
}
