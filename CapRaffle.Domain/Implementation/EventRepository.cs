using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using System.Collections.Specialized;
using System.Data.Objects;
using System.Reflection;
using System.Data.Objects.DataClasses;
using System.Data.Entity;
using CapRaffle.Domain.Draw;
using Rule = CapRaffle.Domain.Model.Rule;

namespace CapRaffle.Domain.Implementation
{
    public class EventRepository : IEventRepository
    {

        CapRaffleContext context = new CapRaffleContext();

        public IQueryable<Event> Events { get { return context.Events; } }

        public IQueryable<UserEvent> Participants { get { return context.UserEvents; } }

        public IQueryable<User> Users { get { return context.Users; } }

        public IQueryable<Rule> AvailableRules { get { return context.Rules; } }

        public IQueryable<UserEvent> EventParticipants(int eventId)
        {
            return context.UserEvents.Where(x => x.EventId == eventId).AsQueryable();
        }

        public IQueryable<Category> Categories
        {
            get { return context.Categories; }
        }

        public void SaveCategory(Category category)
        {
            if (category.CategoryId == 0)
            {
                context.AddToCategories(category);
            }
            else
            {
                try
                {
                    context.UpdateDetachedEntity<Category>(category, x => x.CategoryId);
                }
                catch (Exception)
                {
                    context.ApplyCurrentValues<Category>(category.EntityKey.EntitySetName, category);
                }
            }
            context.SaveChanges();
        }
        
        public void DeleteEvent(Event selectedEvent)
        {
            context.Events.DeleteObject(selectedEvent);
            context.SaveChanges();
        }

        public void SaveEvent(Event changedEvent)
        {
            if (changedEvent.EventId == 0)
            {
                context.AddToEvents(changedEvent);
            }
            else
            {
                context.UpdateDetachedEntity<Event>(changedEvent, x => x.EventId);
            }
            context.SaveChanges();

        }

        public void DeleteParticipant(UserEvent participant)
        {
            context.UpdateDetachedEntity<UserEvent>(participant, x => x.EventId);
            context.UserEvents.DeleteObject(participant);
            context.SaveChanges();
        }

        public void SaveParticipant(UserEvent participant)
        {
            if (DeadlineForEventHasPassed(participant.EventId))
            {
                throw new ArgumentException("You cant create or edit a participation after the deadline");
            }

            if (context.UserEvents.Where(x => x.EventId == participant.EventId && x.UserEmail.Equals(participant.UserEmail)).Count() == 0)
            {
                context.AddToUserEvents(participant);
            }
            else
            {
                context.UpdateDetachedEntity<UserEvent>(participant, x => x.EventId);
            }
            context.SaveChanges();
        }

        public void SaveRulesForCategory(int categoryId, List<RuleParameter> ruleParameters)
        {
            var rulesetId = GetRulesetId(categoryId);
            var priority = 1;
            var existingrules = context.RuleSets.Where(x => x.CateogryId == categoryId).ToList();
            existingrules.ForEach(x => context.RuleSets.DeleteObject(x));
            foreach (var parameter in ruleParameters)
            {
                context.RuleSets.AddObject(new RuleSet { RuleSetId = rulesetId, RuleId = parameter.Rule.RuleId, CateogryId = categoryId, RuleParameter = parameter.Param, Priority = priority });
                priority++;
            }
            context.SaveChanges();
        }

        private int GetRulesetId(int categoryId)
        {
            var rulesets = (from n in context.RuleSets
                            where n.CateogryId == categoryId
                            select n.RuleSetId).ToList();
            int rulesetId = 0;
            if (rulesets.Count() > 0)
            {
                rulesetId = rulesets.FirstOrDefault();
            }
            else
            {
                if (context.RuleSets.Count() > 0)
                {
                    rulesetId = context.RuleSets.OrderByDescending(x => x.RuleSetId).FirstOrDefault().RuleSetId + 1;
                }
                else
                {
                    rulesetId = 1;
                }
            }
            return rulesetId;
        }

        public List<RuleParameter> GetRulesForCategory(int categoryId)
        {
            var ruleSetId = -1;
            if(context.RuleSets.Where(e => e.CateogryId == categoryId).Count() > 0)
                ruleSetId = context.RuleSets.Where(e => e.CateogryId == categoryId).FirstOrDefault().RuleSetId;

            List<RuleSet> ruleSets = context.RuleSets.Where(rs => rs.RuleSetId == ruleSetId).OrderBy(rs => rs.Priority).ToList<RuleSet>();
            List<RuleParameter> ruleList = new List<RuleParameter>();
            foreach (RuleSet rs in ruleSets)
            {
                Rule rule = context.Rules.FirstOrDefault(r => r.RuleId == rs.RuleId);
                int param = (rs.RuleParameter != null) ? (int)rs.RuleParameter : 0;
                ruleList.Add(new RuleParameter { Rule = rule, Param = param });
            }
            return ruleList;
        }

        private bool DeadlineForEventHasPassed(int eventid)
        {
            var selectedEvent = context.Events.Where(x => x.EventId == eventid).FirstOrDefault();
            if (selectedEvent == null)
            {
                return false;
            }
            var test = selectedEvent.DeadLine < DateTime.Now;
            return test;
        }
    }
}
