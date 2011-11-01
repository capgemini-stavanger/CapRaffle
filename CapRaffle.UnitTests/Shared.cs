using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Controllers;
using Moq;
using CapRaffle.Domain.Model;
using System.Web.Mvc;
using CapRaffle.Models;
using System.Web.Routing;
using NUnit.Framework;


namespace CapRaffle.UnitTests
{
    public class Shared
    {

        protected Mock<IEventRepository> eventMock;
        protected Mock<IAccountRepository> accountMock;
        protected Mock<IDrawingRepository> drawingMock;
        protected Mock<ControllerContext> controllerContextMock;

        protected AccountController accountController;
        protected DrawWinnerController drawWinnerController;
        protected EventController eventController;
        protected CategoryController categoryController;
        protected ParticipantController participantController;
        
        protected UserEvent participant;
        protected EventViewModel selectedEvent;
        protected Event newevent;

        [SetUp]
        public void Setup()
        {
            setupEventMock();
            setupAccountMock();
            setupControllerContextMock();
            setupDrawingMock();
            setNewEvent();
            setSelectedEvent();
            setParticipant();
            setupControllers();
        }

        protected void setupEventMock()
        {
            eventMock = new Mock<IEventRepository>();

            eventMock.Setup(m => m.Events).Returns(GetEvents());

            eventMock.Setup(m => m.Participants).Returns(GetParticipants());

            eventMock.Setup(m => m.EventParticipants(It.IsAny<int>())).Returns((int eventId) =>
                {
                    return GetParticipants().Where(s => s.EventId == eventId);
                });

            eventMock.Setup(m => m.Categories).Returns(GetCategories().AsQueryable());

            eventMock.Setup(m => m.Users).Returns(GetUsers());

        }

        protected void setupAccountMock()
        {
            accountMock = new Mock<IAccountRepository>();

            accountMock.Setup(m => m.Authenticate("test@capgemini.com", "pass1234")).Returns(true);

            accountMock.Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            accountMock.Setup(m => m.ChangePassword("test@capgemini.com", It.IsAny<string>())).Returns(true);

            accountMock.Setup(m => m.GetUserByEmail("test@capgemini.com")).Returns(new User() { Email = "testc@capgemini.com" });
            
            accountMock.Setup(m => m.Users).Returns(GetUsers());
        }

        protected void setupControllerContextMock()
        {
            controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("test@capgemini.com");
            controllerContextMock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
        }

        protected void setupDrawingMock()
        {
            drawingMock = new Mock<IDrawingRepository>();
            drawingMock.Setup(m => m.Winners).Returns(GetWinners());
        }

        protected void setupControllers()
        {
            categoryController = new CategoryController(eventMock.Object);
            accountController = new AccountController(accountMock.Object);
            drawWinnerController = new DrawWinnerController(drawingMock.Object);
            eventController = new EventController(eventMock.Object);
            participantController = new ParticipantController(eventMock.Object, accountMock.Object);

            categoryController.ControllerContext = controllerContextMock.Object;
            accountController.ControllerContext = controllerContextMock.Object;
            drawWinnerController.ControllerContext = controllerContextMock.Object;
            eventController.ControllerContext = controllerContextMock.Object;
            participantController.ControllerContext = controllerContextMock.Object;
        }

        private void setSelectedEvent()
        {
            IEnumerable<SelectListItem> categories = GetCategories().ToList().Select(x =>
                new SelectListItem { Text = x.Name, Value = x.CategoryId.ToString() }
                );

            categories.FirstOrDefault().Selected = true;
            selectedEvent = new EventViewModel { SelectedEvent = newevent, Categories = categories };
        }

        private void setNewEvent()
        {
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
        }

        private void setParticipant()
        {
            participant = new UserEvent { EventId = 1, UserEmail = "test@capgemini.com", NumberOfSpots = 1 };
        }

        private static IQueryable<User> GetUsers()
        {
            return new User[] {
                new User { Email = "test@testeland.no", Name = "test", Password = "suppersikkert" },
                new User { Email = "atest@testeland.no", Name = "atest", Password = "suppersikkert" },
                new User { Email = "aaatest@testeland.no", Name = "aaatest", Password = "suppersikkert" },
                new User { Email = "btest@testeland.no", Name = "btest", Password = "suppersikkert" },
                new User { Email="test@capgemini.com", Name="Test" },
                new User { Email="test2@capgemini.com", Name="Test2" }
            }.AsQueryable();
        }

        private static IQueryable<UserEvent> GetParticipants()
        {
            return new UserEvent[] {
                new UserEvent { EventId = 1, UserEmail = "a@capgemini.com", NumberOfSpots = 2 },
                new UserEvent { EventId = 1, UserEmail = "b@capgemini.com", NumberOfSpots = 1 },
                new UserEvent { EventId = 2, UserEmail = "a@capgemini.com", NumberOfSpots = 1 },
                new UserEvent { EventId = 5, UserEmail = "a@capgemini.com", NumberOfSpots = 2 },
                new UserEvent { EventId = 5, UserEmail = "b@capgemini.com", NumberOfSpots = 2 },
                new UserEvent { EventId = 1, UserEmail = "c@capgemini.com", NumberOfSpots = 1 },
                new UserEvent { EventId = 1, UserEmail = "arne.aase@capgemini.com", NumberOfSpots = 1 },
                new UserEvent { EventId = 1, UserEmail = "test@capgemini.com", NumberOfSpots = 1 },
                new UserEvent { EventId = 1, UserEmail = "aaatest@testeland.no", NumberOfSpots = 1 },
                new UserEvent { EventId = 1, UserEmail = "btest@testeland.no", NumberOfSpots = 1 },
            }.AsQueryable();
        }

        private static IQueryable<Event> GetEvents()
        {
            return new Event[] {
                new Event { EventId = 1, Name = "event 1", Created = DateTime.Now, Creator = "creator 1", AvailableSpots = 4, DeadLine = DateTime.Now, CategoryId = 1 },
                new Event { EventId = 2, Name = "event 2", Created = DateTime.Now, Creator = "creator 2", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 2 },
                new Event { EventId = 3, Name = "event 3", Created = DateTime.Now, Creator = "test@capgemini.com", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 3 },
                new Event { EventId = 4, Name = "event 4", Created = DateTime.Now, Creator = "creator 4", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 4 },
                new Event { EventId = 5, Name = "event 5", Created = DateTime.Now, Creator = "creator 5", AvailableSpots = 3, DeadLine = DateTime.Now, CategoryId = 5 },
                new Event { EventId = 6, Name = "event 6", Created = DateTime.Now, Creator = "arne.aase@capgemini.com", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 3 }
            }.AsQueryable();
        }

        private static IQueryable<Category> GetCategories()
        {
            return new Category[] {
                new Category { CategoryId = 1, Name = "Category1", IsActive = true },
                new Category { CategoryId = 2, Name = "Category2", IsActive = true },
                new Category { CategoryId = 3, Name = "Category3", IsActive = true },
                new Category { CategoryId = 4, Name = "Category4", IsActive = true },
                new Category { CategoryId = 5, Name = "Category5", IsActive = true },
                new Category { CategoryId = 6, Name = "Hockey", IsActive = true },
                new Category { CategoryId = 7, Name = "A category", IsActive = true },
                new Category { CategoryId = 8, Name = "Other", IsActive = true }
            }.AsQueryable();
        }

        private static IQueryable<Winner> GetWinners()
        {
            return new Winner[] {
                new Winner { EventId = 1, UserEmail = "a@capgemini.com", NumberOfSpotsWon = 2 },
                new Winner { EventId = 2, UserEmail = "remove@capgemini.com", NumberOfSpotsWon = 2}
            }.AsQueryable();
        }
    }
}
