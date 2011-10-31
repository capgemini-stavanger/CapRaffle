using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.Models;
using CapRaffle.ActionFilterAttributes;
using CapRaffle.Domain.Raffle;

namespace CapRaffle.Controllers
{
    
    [SetSelectedMenu]
    public class DrawWinnerController : Controller
    {
        private IDrawingRepository repository;

        public DrawWinnerController(IDrawingRepository drawRepo)
        {
            repository = drawRepo;
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult DrawWinner(int eventId, string view)
        {
            repository.PerformDrawing(eventId);
            DrawWinnerViewModel model = GenerateDrawWinnerViewModel(eventId);
            return PartialView(view, model);
        }

        public PartialViewResult ReplayRaffle(int eventId, string view)
        {
            DrawWinnerViewModel model = GenerateDrawWinnerViewModel(eventId);
            return PartialView(view, model);
        }

        [Authorize]
        public JsonResult RemoveWinner(Winner winner)
        {
            var selectedEvent = repository.Winners.Where(x => x.EventId == winner.EventId).Select(x => x.Event).FirstOrDefault();
            if (!HttpContext.User.Identity.Name.Equals(selectedEvent.Creator))
            {
                return this.Json(false);
            }
            repository.RemoveWinner(winner);
            return this.Json(true);
        }

        [Authorize]
        public PartialViewResult Rules(int eventId)
        {
            var rulesforevent = repository.GetRulesForEvent(eventId) ?? new List<RuleParameter>();
            var available = repository.AvailableRules.ToList();
            available.RemoveAll(x => rulesforevent.Exists(y => y.Rule.RuleId == x.RuleId));
            var model = new RulesViewModel
            {
                AvailableRules = available,
                RulesForEvent = rulesforevent,
                EventId = eventId
            };
            
            return PartialView("_Rules", model);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveRules(int eventid, List<SaveRuleViewModel> rules)
        {
            var ruleparameters = new List<RuleParameter>();
            if (rules != null)
            {
                rules = rules.Distinct(new RuleComparer()).ToList();
                rules.ForEach(x => ruleparameters.Add(new RuleParameter { Rule = new Rule { RuleId = x.RuleId }, Param = x.Param }));
            }
            repository.SaveRulesForEvent(eventid, ruleparameters);
            return this.Json(true);
        }

        [Authorize]
        public ActionResult NotifyParticipants(int eventid)
        {
            if (repository.NotifyParticipants(eventid))
            {
                this.Success("Winners are now notified");
            }
            else
            {
                this.Error("Something wrong happend, Is the smtp server configured, can it be down? Try again later");
            }
            return RedirectToAction("Details", "Event", new { id = eventid });
        }

        private DrawWinnerViewModel GenerateDrawWinnerViewModel(int eventId)
        {
            DrawWinnerViewModel viewModel = new DrawWinnerViewModel
            {
                Winners = repository.WinnersForEvent(eventId).ToList<Winner>(),
                Participants = repository.EventParticipantsForEvent(eventId).ToList<UserEvent>()
            };
            return viewModel;
        }
    }
}
