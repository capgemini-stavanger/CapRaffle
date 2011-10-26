using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using System.Reflection;
using CapRaffle.Domain.Draw;

namespace CapRaffle.Domain.Implementation
{
    public class DrawingRepository : IDrawingRepository, IDisposable
    {
        public void Dispose()
        {
            context.Dispose();
        }

        private CapRaffleContext context = new CapRaffleContext();

        public IQueryable<Winner> Winners { get { return context.Winners; } }

        public IQueryable<Rule> AvailableRules { get { return context.Rules; } }

        public IQueryable<Winner> WinnersForEvent(int eventId) 
        {
            return context.Winners.Where(w => w.EventId == eventId).AsQueryable<Winner>();
        }

        public IQueryable<UserEvent> EventParticipantsForEvent(int eventId)
        {
            return context.UserEvents.Where(w => w.EventId == eventId).AsQueryable<UserEvent>();
        }

        public void PerformDrawing(int eventId)
        {
            var drawing = new DrawWinners(eventId, context);
            drawing.ExecuteDraw();
        }

        public void RemoveWinner(Winner winner)
        {
            context.UpdateDetachedEntity<Winner>(winner, x => x.EventId);
            context.Winners.DeleteObject(winner);
            context.SaveChanges();
        }

        public void PerformAutomaticDrawing()
        {
            var events = context.Events.Where(x => 
                x.IsAutomaticDrawing && 
                x.DeadLine < DateTime.Now &&
                x.UserEvents.Count() > 0).ToList();
            events.RemoveAll(x => x.Winners.Count() > 0 && (x.Winners.Sum(y => y.NumberOfSpotsWon) - x.AvailableSpots) <= 0);
            foreach (var selectedEvent in events)
            {
                PerformDrawing(selectedEvent.EventId);
                NotifyWinners(selectedEvent.EventId);
            }
        }
        
        public void SaveRulesForEvent(int eventId, List<RuleParameter> ruleparameters)
        {
            var rulesetId = GetRulesetId(eventId);
            DeleteRulesForEvent(eventId);
            var priority = 1;
            foreach (var parameter in ruleparameters)
            {
                context.RuleSets.AddObject(new RuleSet { RuleSetId = rulesetId, RuleId = parameter.Rule.RuleId, EventId = eventId, RuleParameter = parameter.Param, Priority = priority });
                priority++;
            }
            context.SaveChanges();
        }

        private int GetRulesetId(int eventId)
        {
            var rulesets = (from n in context.RuleSets
                                where n.EventId == eventId
                                select n.RuleSetId).ToList();
            var ruleSetId = 0;
            if(rulesets.Count() > 0) {
                ruleSetId = rulesets.FirstOrDefault();
            }
            else {
                if(context.RuleSets.Count() > 0)
                {
                    ruleSetId = context.RuleSets.OrderByDescending(x => x.RuleSetId).FirstOrDefault().RuleSetId + 1;
                }
                else {
                    ruleSetId = 1;
                }
            }
            return ruleSetId;
        }

        public List<RuleParameter> GetRulesForEvent(int eventId)
        {
            int ruleSetId;
            List<RuleParameter> ruleList = new List<RuleParameter>();
            if (context.RuleSets.Where(e => e.EventId == eventId).FirstOrDefault() != null)
            {
                ruleSetId = context.RuleSets.Where(e => e.EventId == eventId).FirstOrDefault().RuleSetId;
            }
            else
            {
                //No custom rules just for this event, get category ruleset.
                int eventCategoryId = context.Events.FirstOrDefault(x => x.EventId == eventId).CategoryId;
                if (context.RuleSets.Where(e => e.CateogryId == eventCategoryId).FirstOrDefault() != null)
                {
                    ruleSetId = context.RuleSets.Where(e => e.CateogryId == eventCategoryId).FirstOrDefault().RuleSetId;
                }
                else { return ruleList; }
            }

            List<RuleSet> ruleSets = context.RuleSets.Where(rs => rs.RuleSetId == ruleSetId).OrderBy(rs => rs.Priority).ToList<RuleSet>();
            
            foreach (RuleSet rs in ruleSets)
            {
                Rule rule = context.Rules.FirstOrDefault(r => r.RuleId == rs.RuleId);
                int param = (rs.RuleParameter != null) ? (int)rs.RuleParameter : 0;
                ruleList.Add(new RuleParameter { Rule = rule, Param = param });
            }
            return ruleList;
        }

        private void DeleteRulesForEvent(int eventId)
        {
            var existingrules = context.RuleSets.Where(x => x.EventId == eventId).ToList();
            existingrules.ForEach(x => context.RuleSets.DeleteObject(x));
        }

        public bool NotifyWinners(int eventId)
        {
            IEmailSender emailSender = new EmailSender();
            bool emailsDeliverd = true;
            foreach(Winner w in WinnersForEvent(eventId))
            {
                bool emailSendt = emailSender.NotifyWinner(w);
                if (!emailSendt) emailsDeliverd = false;
            }
            return emailsDeliverd;
        }


    }
}
