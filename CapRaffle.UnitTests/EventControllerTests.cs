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
using CapRaffle.HtmlHelpers;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    public class EventControllerTests : Shared
    {
      
        
        #region create
        
        [Test]
        public void Can_Get_Correct_Create_View()
        {
            //Arrange
            //Act
            var result = eventController.Create();
            //Assert
            result.AssertViewRendered().ForView("EventForm");
        }

        [Test]
        public void Can_Create_New_Events()
        {
            //Arrange

            //Act
            var result = eventController.Create(selectedEvent);

            //Assert
            eventMock.Verify(m => m.SaveEvent(newevent));
            result.AssertActionRedirect().ToAction("Index");
            
        }

        [Test]
        public void Can_Not_Save_Invalid_Changes_On_Events()
        {
            //Arrange
            eventController.ModelState.AddModelError("error", "error");

            //Act
            var result = eventController.Create(selectedEvent);
            
            //Assert            
            result.AssertViewRendered().ForView("EventForm");
            eventMock.Verify(m => m.SaveEvent(It.IsAny<Event>()), Times.Never());
        }

        #endregion


        [Test]
        public void Can_Return_Correct_Event_On_Details_Action()
        {
            //Arrange

            //Act
            ViewResult result = (ViewResult)eventController.Details(1);
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
            var result = eventController.Delete(3);

            //Assert
            result.AssertActionRedirect().ToAction("Index");
            eventMock.Verify(m => m.DeleteEvent(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void User_Can_Not_Delete_Others_Event()
        {
            //Arrange
            //Act
            var result = eventController.Delete(4);

            //Assert
            result.AssertActionRedirect().ToAction("Details");
            eventMock.Verify(m => m.DeleteEvent(It.IsAny<Event>()), Times.Never());
        }

        [Test]
        public void User_Can_Edit_Events_The_User_Created()
        {
            //Arrange
            //Act
            var result = (ViewResult)eventController.CreateBasedOnOldEvent(2);
            
            //Assert
            result.AssertViewRendered().ForView("EventForm");
        }

        [Test]
        public void User_Can_Create_New_Event_Based_On_Old_Event()
        {
            //Arrange
            //Act
            var result = (ViewResult)eventController.Edit(3);

            //Assert
            result.AssertViewRendered().ForView("EventForm");
        }

        [Test]
        public void User_Can_Not_Edit_Others_Event()
        {
            //Act
            var result = eventController.Edit(4);

            //Assert
            Assert.IsNotNull(eventController.TempData["Info"]);
            result.AssertActionRedirect().ToAction("Details");
        }

        [Test]
        public void User_Can_Submit_Valid_Changes_On_Event()
        {
            //Arrange
            selectedEvent.SelectedEvent.DeadLine.AddHours(2);
            //Act
            var result = eventController.Edit(selectedEvent);

            //Assert
            Assert.IsNotNull(eventController.TempData["Success"]);
            result.AssertActionRedirect().ToAction("Details");
            eventMock.Verify(m => m.SaveEvent(selectedEvent.SelectedEvent), Times.Once());
        }

        [Test]
        public void User_Can_Not_Submit_Invalid_Changes_On_Event()
        {
            //Arrange
            eventController.ModelState.AddModelError("error", "error");

            //Act
            var result = eventController.Edit(selectedEvent);

            //Assert
            result.AssertViewRendered().ForView("EventForm");
            eventMock.Verify(m => m.SaveEvent(It.IsAny<Event>()), Times.Never());
        }

        [Test]
        public void Can_Paginate()
        {
            eventController.PageSize = 3;

            ActionResult res = eventController.Index(true, 2);
            ViewResult wr = (ViewResult)res;
            EventsListViewModel elvm = (EventsListViewModel)wr.ViewData.Model;
            Event[] events = elvm.Events.ToArray();


            // Assert - Should be 3 events.
            Assert.IsTrue(events.Length == 3);
            Assert.AreEqual(events[0].Name, "event 4");
            Assert.AreEqual(events[1].Name, "event 5");
        }
    }
}
