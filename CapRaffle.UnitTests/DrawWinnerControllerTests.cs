using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using System.Web.Mvc;
using CapRaffle.Models;
using CapRaffle.Controllers;
using MvcContrib.TestHelper;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    public class DrawWinnerControllerTests
    {
        Mock<IEventRepository> eventMock;
        Mock<IDrawingRepository> drawingMock;
        EventViewModel selectedEvent;
        DrawWinnerController controller;

        [SetUp]
        public void Setup()
        {
            var mockHttpContext = new Mock<ControllerContext>();

            mockHttpContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("creator 2");
            mockHttpContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            eventMock = new Mock<IEventRepository>();
            eventMock.Setup(m => m.Events).Returns(new Event[] {
                new Event { EventId = 1, Name = "event 1", Created = DateTime.Now, Creator = "creator 1", AvailableSpots = 4, DeadLine = DateTime.Now, CategoryId = 1 },
                new Event { EventId = 2, Name = "event 2", Created = DateTime.Now, Creator = "creator 2", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 2 },
                new Event { EventId = 3, Name = "event 3", Created = DateTime.Now, Creator = "creator 3", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 3 },
                new Event { EventId = 4, Name = "event 4", Created = DateTime.Now, Creator = "creator 4", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 4 },
                new Event { EventId = 5, Name = "event 5", Created = DateTime.Now, Creator = "creator 5", AvailableSpots = 3, DeadLine = DateTime.Now, CategoryId = 5 }
            }.AsQueryable());

            eventMock.Setup(m => m.Participants).Returns(new UserEvent[] {
                new UserEvent { EventId = 1, UserEmail = "a@capgemini.com", NumberOfSpots = 2 },
                new UserEvent { EventId = 1, UserEmail = "b@capgemini.com", NumberOfSpots = 1 },
                new UserEvent { EventId = 2, UserEmail = "a@capgemini.com", NumberOfSpots = 1 },
                new UserEvent { EventId = 5, UserEmail = "a@capgemini.com", NumberOfSpots = 2 },
                new UserEvent { EventId = 5, UserEmail = "b@capgemini.com", NumberOfSpots = 2 },
                new UserEvent { EventId = 1, UserEmail = "c@capgemini.com", NumberOfSpots = 1 }
            }.AsQueryable());

            eventMock.Setup(m => m.EventParticipants(It.IsAny<int>()))
                .Returns((int eventId) => { 
                    return eventMock.Object.Participants.Where(s => s.EventId == eventId); 
                });

            eventMock.Setup(m => m.Categories).Returns(new Category[] {
                new Category { CategoryId = 1, Name = "Category1" },
                new Category { CategoryId = 2, Name = "Category2" },
                new Category { CategoryId = 3, Name = "Category3" },
                new Category { CategoryId = 4, Name = "Category4" },
                new Category { CategoryId = 5, Name = "Category5" }
            }.AsQueryable());

            IEnumerable<SelectListItem> categories = eventMock.Object.Categories.ToList().Select(x =>
                new SelectListItem { Text = x.Name, Value = x.CategoryId.ToString() }
                );
            drawingMock = new Mock<IDrawingRepository>();
            drawingMock.Setup(m => m.Winners).Returns(new Winner[] {
                new Winner { EventId = 1, UserEmail = "a@capgemini.com", NumberOfSpotsWon = 2 },
                new Winner { EventId = 2, UserEmail = "remove@capgemini.com", NumberOfSpotsWon = 2}
            }.AsQueryable());
            categories.FirstOrDefault().Selected = true;

            selectedEvent = new EventViewModel { SelectedEvent = eventMock.Object.Events.FirstOrDefault(), Categories = categories };

            controller = new DrawWinnerController(drawingMock.Object);
        }

        [Test]
        public void Can_Draw_Winners_For_All_Available_Spots()
        {
            PartialViewResult result = (PartialViewResult)controller
                .DrawWinner(SelectedEventId(), "Default");

            DrawWinnerViewModel viewModel = (DrawWinnerViewModel)result.Model;

            Assert.IsInstanceOf(typeof(DrawWinnerViewModel), result.Model);
            Assert.AreEqual(0, viewModel.NumberOfSpotsLeft);
            drawingMock.Verify(m => m.PerformDrawing(It.IsAny<int>()), Times.AtLeastOnce());
            result.AssertPartialViewRendered().ForView("Default");
        }

        [Test]
        public void Can_Draw_Winners_When_Wanted_Spots_Is_Higher_Than_Remaining_Spots()
        {
            PartialViewResult result = (PartialViewResult)controller
                .DrawWinner(5, "Default");

            DrawWinnerViewModel viewModel = (DrawWinnerViewModel)result.Model;

            Assert.IsInstanceOf(typeof(DrawWinnerViewModel), result.Model);
            Assert.AreEqual(0, viewModel.NumberOfSpotsLeft);
            drawingMock.Verify(m => m.PerformDrawing(It.IsAny<int>()), Times.AtLeastOnce());
            result.AssertPartialViewRendered().ForView("Default");
        }

        [Test]
        public void Can_Display_Event_Rules()
        {
            var result = controller.Rules(1);

            result.AssertPartialViewRendered().ForView("_Rules");
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }

        [Test]
        public void Can_Save_Event_Rules()
        {
            List<SaveRuleViewModel> rules = new List<SaveRuleViewModel>();
            SaveRuleViewModel srvm = new SaveRuleViewModel { RuleId = 1, Param = 10 };
            rules.Add(srvm);

            var result = controller.SaveRules(1, rules);

            Assert.IsInstanceOf(typeof(JsonResult), result);
        }

        [Test]
        public void Can_Notify_Winners()
        {
            drawingMock.Setup(m => m.NotifyWinners(1)).Returns(true);

            var result = controller.NotifyWinners(1);

            drawingMock.Verify(m => m.NotifyWinners(1), Times.AtLeastOnce());
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void Can_Not_Notify_Winners()
        {

            drawingMock.Setup(m => m.NotifyWinners(2)).Returns(false);

            var result = controller.NotifyWinners(2);

            drawingMock.Verify(m => m.NotifyWinners(2), Times.AtLeastOnce());
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        private IEnumerable<UserEvent> SelectedEventParticipants()
        {
            return eventMock.Object.Participants
                .Where(x => x.EventId == SelectedEventId());
        }

        private int SelectedEventId()
        {
            return selectedEvent.SelectedEvent.EventId;
        }
    }
}
