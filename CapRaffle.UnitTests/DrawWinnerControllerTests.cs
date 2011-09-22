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
        Mock<ICategoryRepository> categoryMock;
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
                new Event { EventId = 1, Name = "event 1", Created = DateTime.Now, Creator = "creator 1", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 1 },
                new Event { EventId = 2, Name = "event 2", Created = DateTime.Now, Creator = "creator 2", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 2 },
                new Event { EventId = 3, Name = "event 3", Created = DateTime.Now, Creator = "creator 3", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 3 },
                new Event { EventId = 4, Name = "event 4", Created = DateTime.Now, Creator = "creator 4", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 4 },
                new Event { EventId = 5, Name = "event 5", Created = DateTime.Now, Creator = "creator 5", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 5 }
            }.AsQueryable());

            eventMock.Setup(m => m.Participants).Returns(new UserEvent[] {
                new UserEvent { EventId = 1, UserEmail = "a@capgemini.com", NumberOfSpots = 2 },
                new UserEvent { EventId = 1, UserEmail = "b@capgemini.com", NumberOfSpots = 1 },
                new UserEvent { EventId = 2, UserEmail = "a@capgemini.com", NumberOfSpots = 1 },
                new UserEvent { EventId = 1, UserEmail = "c@capgemini.com", NumberOfSpots = 1 }
            }.AsQueryable());

            eventMock.Setup(m => m.EventParticipants(It.IsAny<int>()))
                .Returns((int eventId) => { 
                    return eventMock.Object.Participants.Where(s => s.EventId == eventId); 
                });

            categoryMock = new Mock<ICategoryRepository>();
            categoryMock.Setup(m => m.Categories).Returns(new Category[] {
                new Category { CategoryId = 1, Name = "Category1" },
                new Category { CategoryId = 2, Name = "Category2" },
                new Category { CategoryId = 3, Name = "Category3" },
                new Category { CategoryId = 4, Name = "Category4" },
                new Category { CategoryId = 5, Name = "Category5" }
            }.AsQueryable());

            IEnumerable<SelectListItem> categories = categoryMock.Object.Categories.ToList().Select(x =>
                new SelectListItem { Text = x.Name, Value = x.CategoryId.ToString() }
                );

            categories.FirstOrDefault().Selected = true;

            selectedEvent = new EventViewModel { SelectedEvent = eventMock.Object.Events.FirstOrDefault(), Categories = categories };

            controller = new DrawWinnerController(eventMock.Object, categoryMock.Object);
        }

        [Test]
        public void Has_All_Participants_For_Current_Event()
        {
            ViewResult result = (ViewResult)controller.Index(selectedEvent.SelectedEvent.EventId);

            IQueryable<UserEvent> participants = result.Model as IQueryable<UserEvent>;

            var correctNumberOfParticipants = eventMock.Object.Participants
                .Where(x => x.EventId == selectedEvent.SelectedEvent.EventId).Count();

            Assert.AreEqual(correctNumberOfParticipants, participants.Count());
            result.AssertViewRendered().ForView(string.Empty);
            eventMock.Verify(m => m.EventParticipants(selectedEvent.SelectedEvent.EventId), Times.Once());
        }
    }
}
