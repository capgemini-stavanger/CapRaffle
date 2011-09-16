using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CapRaffle.Domain.Abstract;
using Moq;
using CapRaffle.Domain.Model;
using CapRaffle.Controllers;
using System.Web.Mvc;
using Event = CapRaffle.Domain.Model.Event;
using MvcContrib.TestHelper;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    public class EventRepositoryTests
    {
        Mock<IEventRepository> mock;
        Event newevent;
        EventController controller;

        [SetUp]
        public void setup()
        {
            mock = new Mock<IEventRepository>();
            mock.Setup(m => m.Events).Returns(new Event[] {
                new Event { EventId = 1, Name = "event 1", Created = DateTime.Now, Creator = "creator 1", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 1 },
                new Event { EventId = 2, Name = "event 2", Created = DateTime.Now, Creator = "creator 2", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 2 },
                new Event { EventId = 3, Name = "event 3", Created = DateTime.Now, Creator = "creator 3", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 3 },
                new Event { EventId = 4, Name = "event 4", Created = DateTime.Now, Creator = "creator 4", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 4 },
                new Event { EventId = 5, Name = "event 5", Created = DateTime.Now, Creator = "creator 5", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 5 }
            }.AsQueryable());
            

            newevent = new Event
            {
                EventId = 10,
                Name = "CanCreateNewEvents",
                Created = DateTime.Now,
                Creator = "CanCreateNewEventsTest",
                AvailableSpots = 2,
                DeadLine = DateTime.Now,
                CategoryId = 10
            };

            controller = new EventController(mock.Object);
        }

        [Test]
        public void Can_Create_New_Events()
        {
            //Arrange

            //Act
            var result = controller.Create(newevent);

            //Assert
            result.AssertActionRedirect().ToAction("Index");
            mock.Verify(m => m.SaveEvent(newevent));
        }

        [Test]
        public void Cannot_Save_Invalid_Changes()
        {
            //Arrange
            controller.ModelState.AddModelError("error", "error");

            //Act
            var result = controller.Create(newevent);
            
            //Assert            
            result.AssertViewRendered().ForView(string.Empty);
            mock.Verify(m => m.SaveEvent(It.IsAny<Event>()), Times.Never());
        }

    }


}
