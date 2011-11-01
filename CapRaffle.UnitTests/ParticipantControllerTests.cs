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
    class ParticipantControllerTests : Shared
    {
        

        [Test]
        public void Can_Get_Json_Result()
        {
            //Arrange
            
            //Act
            ActionResult result = participantController.Create(participant);

            //Assert
            //Assert.AreEqual(true, result.Data);
            eventMock.Verify(m => m.SaveParticipant(participant), Times.Once());
        }

        [Test]
        public void Can_Delete_Own_Participation()
        {
            //Arrange
            //Act
            JsonResult result = participantController.Delete(participant);
            
            //Assert
            Assert.AreEqual(true, result.Data);
            eventMock.Verify(m => m.DeleteParticipant(participant), Times.Once());
        }

        [Test]
        public void Creator_can_delete_other_participants()
        {
            participant.UserEmail = "test@testeland.no";
            participant.EventId = 3;

            JsonResult result = participantController.Delete(participant);

            Assert.AreEqual(true, result.Data);
            eventMock.Verify(m => m.DeleteParticipant(participant), Times.Once());
        }

        [Test]
        public void Creator_Can_Create_Participants()
        {
            participant.UserEmail = "finnesikke@blirderforlaget.no";

            var result = participantController.Create(participant);

            Assert.IsInstanceOf(typeof(ActionResult), result);
            accountMock.Verify(m => m.Create(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()), Times.Once());
        }

        [Test]
        public void Can_Cast_Exception_On_Create_Participants()
        {
            participant.UserEmail = "finnesikke@blirderforlaget.no";
            eventMock.Setup(em => em.Users).Throws<Exception>();
            var result = participantController.Create(participant);

            Assert.IsInstanceOf(typeof(ActionResult), result);
        }

        [Test]
        public void Can_Not_Delete_Others_Participation()
        {
            //Arrange
            participant.UserEmail = "test@testeland.no";
            //Act
            JsonResult result = participantController.Delete(participant);

            //Assert
            Assert.AreEqual(false, result.Data);
            eventMock.Verify(m => m.DeleteParticipant(participant), Times.Never());
        }

        [Test]
        public void can_get_JSON_Result_With_Users()
        {
            //arrange

            //act
            JsonResult result = participantController.GetUsers("aa");
            List<string> list = (List<string>)result.Data;

            //assert
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, list.Count);
        }
        [Test]
        public void Can_Get_PartialViewResult_Result_With_Participants()
        {
            //arrange

            //act
            var result = participantController.GetParticipants(1);

            //assert
            result.AssertPartialViewRendered().ForView("_GetParticipants");
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }
    }
}
