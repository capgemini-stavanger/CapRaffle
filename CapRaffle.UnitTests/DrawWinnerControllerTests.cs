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
    public class DrawWinnerControllerTests : Shared
    {
        
        [Test]
        public void Can_Draw_Winners_For_All_Available_Spots()
        {
            PartialViewResult result = (PartialViewResult)drawWinnerController
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
            PartialViewResult result = (PartialViewResult)drawWinnerController
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
            var result = drawWinnerController.Rules(1);

            result.AssertPartialViewRendered().ForView("_Rules");
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }

        [Test]
        public void Can_Save_Event_Rules()
        {
            List<SaveRuleViewModel> rules = new List<SaveRuleViewModel>();
            SaveRuleViewModel srvm = new SaveRuleViewModel { RuleId = 1, Param = 10 };
            rules.Add(srvm);

            var result = drawWinnerController.SaveRules(1, rules);

            Assert.IsInstanceOf(typeof(JsonResult), result);
        }

        [Test]
        public void Can_Notify_Winners()
        {
            drawingMock.Setup(m => m.NotifyParticipants(1)).Returns(true);

            var result = drawWinnerController.NotifyParticipants(1);

            drawingMock.Verify(m => m.NotifyParticipants(1), Times.AtLeastOnce());
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void Can_Not_Notify_Winners()
        {

            drawingMock.Setup(m => m.NotifyParticipants(2)).Returns(false);

            var result = drawWinnerController.NotifyParticipants(2);

            drawingMock.Verify(m => m.NotifyParticipants(2), Times.AtLeastOnce());
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
