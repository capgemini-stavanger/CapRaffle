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
using CapRaffle.Models;
using System.Web.Routing;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    public class EventControllerTests
    {
        Mock<IEventRepository> mock;
        Mock<ICategoryRepository> categoryMock;
        Event newevent;
        EventViewModel selectedEvent;
        EventController controller;

        [SetUp]
        public void setup()
        {
            //Arrange
            var mockHttpContext = new Mock<ControllerContext>();

            mockHttpContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("creator 2");
            mockHttpContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            mock = new Mock<IEventRepository>();
            mock.Setup(m => m.Events).Returns(new Event[] {
                new Event { EventId = 1, Name = "event 1", Created = DateTime.Now, Creator = "creator 1", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 1 },
                new Event { EventId = 2, Name = "event 2", Created = DateTime.Now, Creator = "creator 2", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 2 },
                new Event { EventId = 3, Name = "event 3", Created = DateTime.Now, Creator = "creator 3", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 3 },
                new Event { EventId = 4, Name = "event 4", Created = DateTime.Now, Creator = "creator 4", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 4 },
                new Event { EventId = 5, Name = "event 5", Created = DateTime.Now, Creator = "creator 5", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 5 }
            }.AsQueryable());

            categoryMock = new Mock<ICategoryRepository>();
            categoryMock.Setup(m => m.Categories).Returns(new Category[] {
                new Category { CategoryId = 1, Name = "Category1" },
                new Category { CategoryId = 2, Name = "Category2" },
                new Category { CategoryId = 3, Name = "Category3" },
                new Category { CategoryId = 4, Name = "Category4" },
                new Category { CategoryId = 5, Name = "Category5" }
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

            IEnumerable<SelectListItem> categories = categoryMock.Object.Categories.ToList().Select(x =>
                new SelectListItem { Text = x.Name, Value = x.CategoryId.ToString() }
                );

            categories.FirstOrDefault().Selected = true;

            selectedEvent = new EventViewModel { SelectedEvent = newevent, Categories = categories };



            controller = new EventController(mock.Object, categoryMock.Object);
            controller.ControllerContext = new ControllerContext(mockHttpContext.Object.HttpContext, new RouteData(), controller);
        }
        
        #region create
        
        [Test]
        public void Can_Get_Correct_Create_View()
        {
            //Arrange
            //Act
            var result = controller.Create();
            //Assert
            result.AssertViewRendered().ForView("EventForm");
        }

        [Test]
        public void Can_Create_New_Events()
        {
            //Arrange

            //Act
            var result = controller.Create(selectedEvent);

            //Assert
            mock.Verify(m => m.SaveEvent(newevent));
            result.AssertActionRedirect().ToAction("Index");
            
        }

        [Test]
        public void Can_Not_Save_Invalid_Changes_On_Events()
        {
            //Arrange
            controller.ModelState.AddModelError("error", "error");

            //Act
            var result = controller.Create(selectedEvent);
            
            //Assert            
            result.AssertViewRendered().ForView("EventForm");
            mock.Verify(m => m.SaveEvent(It.IsAny<Event>()), Times.Never());
        }

        #endregion

        [Test]
        public void Index_Get_Sorted_List_Of_Events()
        {
            //Arrange

            //Act
            ViewResult result = (ViewResult) controller.Index();
            var events = ((EventsListViewModel)result.Model).Events.ToList();
            

            //Assert
            Assert.IsTrue(events.Count == 5);
            Assert.AreEqual(events.First().EventId, 5);
            Assert.AreEqual(events.Last().EventId, 1);
            result.AssertViewRendered().ForView(string.Empty);
        }

        [Test]
        public void Can_Return_Correct_Event_On_Details_Action()
        {
            //Arrange
            
            
            //Act
            ViewResult result = (ViewResult)controller.Details(1);
            EventViewModel model = (EventViewModel)result.Model;
            
            //Assert
            result.AssertViewRendered().ForView(string.Empty);
            Assert.IsNull(model.LoggedInParticipant);
            Assert.AreEqual(1, model.SelectedEvent.EventId);
            Assert.AreEqual(null, model.Categories);
        }

        [Test]
        public void User_Can_Delete_Events_The_User_Created()
        {
            //Arrange

            //Act
            var result = controller.Delete(2);

            //Assert
            result.AssertActionRedirect().ToAction("Index");
            mock.Verify(m => m.DeleteEvent(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void User_Can_Not_Delete_Others_Event()
        {
            //Arrange
            //Act
            var result = controller.Delete(4);

            //Assert
            result.AssertActionRedirect().ToAction("Details");
            mock.Verify(m => m.DeleteEvent(It.IsAny<Event>()), Times.Never());
        }

        [Test]
        public void User_Can_Edit_Events_The_User_Created()
        {
            //Arrange
            //Act
            var result = (ViewResult)controller.CreateBasedOnOldEvent(2);
            
            //Assert
            result.AssertViewRendered().ForView("EventForm");
        }

        [Test]
        public void User_Can_Create_New_Event_Based_On_Old_Event()
        {
            //Arrange
            //Act
            var result = (ViewResult)controller.Edit(2);

            //Assert
            result.AssertViewRendered().ForView("EventForm");
        }

        [Test]
        public void User_Can_Not_Edit_Others_Event()
        {
            //Act
            var result = controller.Edit(4);

            //Assert
            Assert.IsNotNull(controller.TempData["Info"]);
            result.AssertActionRedirect().ToAction("Details");
        }

        [Test]
        public void User_Can_Submit_Valid_Changes_On_Event()
        {
            //Arrange
            selectedEvent.SelectedEvent.DeadLine.AddHours(2);
            //Act
            var result = controller.Edit(selectedEvent);

            //Assert
            Assert.IsNotNull(controller.TempData["Success"]);
            result.AssertActionRedirect().ToAction("Details");
            mock.Verify(m => m.SaveEvent(selectedEvent.SelectedEvent), Times.Once());
        }

        [Test]
        public void User_Can_Not_Submit_Invalid_Changes_On_Event()
        {
            //Arrange
            controller.ModelState.AddModelError("error", "error");

            //Act
            var result = controller.Edit(selectedEvent);

            //Assert
            result.AssertViewRendered().ForView("EventForm");
            mock.Verify(m => m.SaveEvent(It.IsAny<Event>()), Times.Never());
        }
    }


}
