using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.Domain.Rules;
using System.Reflection;

namespace CapRaffle.Domain.Implementation
{
    public class DrawingRepository : IDrawingRepository
    {
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
            List<UserTickets> userTicketsList = GenerateUserRaffleTicketsList(eventId);
            ApplyRulesForEvent(eventId, userTicketsList);
            List<UserEvent> raffle = GenerateRaffleTickets(userTicketsList , eventId);

            Random randomGenerator = new Random();
            int availableSpots = NumberOfSpotsLeftForEvent(eventId);

            while (availableSpots > 0 && raffle.Count > 0)
            {
                int winnerNumber = randomGenerator.Next(raffle.Count());
                UserEvent drawnParticipant = raffle.ElementAt(winnerNumber);

                int numberOfSpotsGiven = CalculateNumberOfSpotsToGive(
                    availableSpots,
                    drawnParticipant.NumberOfSpots
                    );

                Winner winner = new Winner
                {
                    EventId = drawnParticipant.EventId,
                    UserEmail = drawnParticipant.UserEmail,
                    Date = DateTime.Now,
                    NumberOfSpotsWon = numberOfSpotsGiven,
                    CatogoryId = CategoryIdForEvent(eventId)
                };

                raffle.RemoveAll(x => x.UserEmail == winner.UserEmail);

                SaveWinner(winner);
                DeleteParticipant(drawnParticipant);
                availableSpots -= winner.NumberOfSpotsWon;
            }
        }

        public void RemoveWinner(Winner winner)
        {
            context.UpdateDetachedEntity<Winner>(winner, x => x.EventId);
            context.Winners.DeleteObject(winner);
            context.SaveChanges();
        }

        public int NumberOfSpotsLeftForEvent(int eventId)
        {
            int eventAvailableSpots = context.Events.FirstOrDefault(x => x.EventId == eventId).AvailableSpots;
            List<Winner> winners = context.Winners.Where(x => x.EventId == eventId).ToList<Winner>();
            
            int spotsAlreadyWon = 0;
            foreach(Winner w in winners) 
            {
                spotsAlreadyWon += w.NumberOfSpotsWon;
            }
            
            int actualAvailableSpots = eventAvailableSpots - spotsAlreadyWon;
            return actualAvailableSpots;
        }
        
        public void SaveRulesForEvent(int eventId, List<RuleParameter> ruleparameters)
        {
            var rulesets = (from n in context.RuleSets
                             where n.EventId == eventId
                             select n.RuleSetId);
            DeleteRulesForEvent(eventId);
            var ruleSetId = rulesets.Count() > 0 ? rulesets.FirstOrDefault() : context.RuleSets.OrderByDescending(x => x.RuleSetId).FirstOrDefault().RuleSetId + 1;
            var priority = 1;
            foreach (var parameter in ruleparameters)
            {
                context.RuleSets.AddObject(new RuleSet { RuleSetId = ruleSetId, RuleId = parameter.Rule.RuleId, EventId = eventId, RuleParameter = parameter.Param, Priority = priority });
                priority++;
            }
            context.SaveChanges();
        }

        public List<RuleParameter> GetRulesForEvent(int eventId)
        {
            int ruleSetId;
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
                else { return null; }
            }

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

        private void DeleteRulesForEvent(int eventId)
        {
            var existingrules = context.RuleSets.Where(x => x.EventId == eventId).ToList();
            existingrules.ForEach(x => context.RuleSets.DeleteObject(x));
        }

        private int CategoryIdForEvent(int eventId)
        {
            return context.Events.FirstOrDefault(x => x.EventId == eventId).CategoryId;
        }

        private void SaveWinner(Winner winner)
        {
            if (context.Winners.Where(x => x.EventId == winner.EventId && x.UserEmail == winner.UserEmail).Count() == 0)
            {
                context.AddToWinners(winner);
            }
            else
            {
                context.UpdateDetachedEntity<Winner>(winner, x => x.EventId);
            }
            context.SaveChanges();
        }

        private void DeleteParticipant(UserEvent participant)
        {
            context.UserEvents.DeleteObject(participant);
            context.SaveChanges();
        }

        private int CalculateNumberOfSpotsToGive(int spotsLeft, int wantedSpots)
        {
            int spotsToGive = 0;
            if (wantedSpots > spotsLeft) spotsToGive = spotsLeft;
            if (wantedSpots <= spotsLeft) spotsToGive = wantedSpots;
            return spotsToGive;
        }

        private List<UserTickets> GenerateUserRaffleTicketsList(int eventId)
        {
            List<UserEvent> eventParticipants = EventParticipantsForEvent(eventId).ToList<UserEvent>();
            List<UserTickets> userTicketsList = new List<UserTickets>();
            foreach (UserEvent ue in eventParticipants)
            {
                UserTickets urt = new UserTickets(ue.UserEmail, 100);
                userTicketsList.Add(urt);
            }
            return userTicketsList;
        }

        private List<UserEvent> GenerateRaffleTickets(List<UserTickets> urt, int eventId)
        {
            List<UserEvent> raffle = new List<UserEvent>();
            foreach (UserTickets ut in urt)
            {
                UserEvent userEvent = context.UserEvents.FirstOrDefault(x => x.UserEmail.Equals(ut.Email) && x.EventId == eventId);
                for (int i = 0; i < ut.NumberOfTickets; i++)
                {
                    raffle.Add(userEvent);
                }
            }
            return raffle;
        }

        private void ApplyRulesForEvent(int eventId, List<UserTickets> userTicketsList)
        {
            List<RuleParameter> rules = GetRulesForEvent(eventId);
            foreach (RuleParameter rp in rules)
            {
                InvokeRuleMethod(rp, userTicketsList, eventId);
            }
        }

        private void InvokeRuleMethod(RuleParameter ruleParameter, List<UserTickets> userTicketsList, int eventId)
        {
            Assembly MyAssembly = Assembly.Load("CapRaffle.Domain");
            Type calledType = MyAssembly.GetType("CapRaffle.Domain.Rules."+ruleParameter.Rule.ClassName);
            if (calledType != null)
            {
                int catId = CategoryIdForEvent(eventId);
                object MyObj = Activator.CreateInstance(calledType, catId);
                calledType.InvokeMember(
                    ruleParameter.Rule.MethodName,
                    BindingFlags.InvokeMethod | BindingFlags.Default,
                    null,
                    MyObj,
                    new Object[] { userTicketsList, ruleParameter.Param });
            }
        }  
    }
}
