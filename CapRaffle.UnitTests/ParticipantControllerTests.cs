using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Models;
using Moq;
using CapRaffle.Domain.Abstract;
using CapRaffle.Controllers;
using NUnit.Framework;
using CapRaffle.Domain.Model;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    class ParticipantControllerTests
    {
        Mock<IEventRepository> mock;
        ParticipantController controller;
        UserEvent participant; 

        [SetUp]
        public void setup()
        {
            //Arrange
            var mockHttpContext = new Mock<ControllerContext>();

            mockHttpContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("arne.aase@capgemini.com");
            mockHttpContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            mock = new Mock<IEventRepository>();
            mock.Setup(m => m.Events).Returns(new Event[] {
                new Event { EventId = 1, Name = "event 1", Created = DateTime.Now, Creator = "creator 1", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 1 },
                new Event { EventId = 2, Name = "event 2", Created = DateTime.Now, Creator = "creator 2", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 2 },
                new Event { EventId = 3, Name = "event 3", Created = DateTime.Now, Creator = "creator 3", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 3 },
                new Event { EventId = 4, Name = "event 4", Created = DateTime.Now, Creator = "creator 4", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 4 },
                new Event { EventId = 5, Name = "event 5", Created = DateTime.Now, Creator = "creator 5", AvailableSpots = 2, DeadLine = DateTime.Now, CategoryId = 5 }
            }.AsQueryable());

            mock.Setup(m => m.Users).Returns(new User[] {
                new User { Email = "test@testeland.no", Name = "test", Password = "suppersikkert" },
                new User { Email = "atest@testeland.no", Name = "atest", Password = "suppersikkert" },
                new User { Email = "aaatest@testeland.no", Name = "aaatest", Password = "suppersikkert" },
                new User { Email = "btest@testeland.no", Name = "btest", Password = "suppersikkert" }
            }.AsQueryable());

            var accountmock = new Mock<IAccountRepository>();

            participant = new UserEvent { EventId = 1, UserEmail = "arne.aase@capgemini.com", NumberOfSpots = 1 };

            controller = new ParticipantController(mock.Object, accountmock.Object);
            controller.ControllerContext = new ControllerContext(mockHttpContext.Object.HttpContext, new RouteData(), controller);
        }

        [Test]
        public void Can_Get_Json_Result()
        {
            //Arrange
            
            //Act
            ActionResult result = controller.Create(participant);

            //Assert
            //Assert.AreEqual(true, result.Data);
            mock.Verify(m => m.SaveParticipant(participant), Times.Once());
        }

        [Test]
        public void Can_Delete_Own_Participation()
        {
            //Arrange
            //Act
            JsonResult result = controller.Delete(participant);
            
            //Assert
            Assert.AreEqual(true, result.Data);
            mock.Verify(m => m.DeleteParticipant(participant), Times.Once());
        }

        [Test]
        public void Can_Not_Delete_Others_Participation()
        {
            //Arrange
            participant.UserEmail = "test@testeland.no";
            //Act
            JsonResult result = controller.Delete(participant);

            //Assert
            Assert.AreEqual(false, result.Data);
            mock.Verify(m => m.DeleteParticipant(participant), Times.Never());
        }

        [Test]
        public void can_get_JSON_Result_With_Users()
        {
            //arrange

            //act
            JsonResult result = controller.GetUsers("aa");
            List<string> list = (List<string>)result.Data;

            //assert
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, list.Count);
        }
    }
}
