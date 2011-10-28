using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;
using CapRaffle.Domain.Implementation;
using System.Reflection;

namespace CapRaffle.Domain.Raffle
{
    public class DrawWinners
    {
        CapRaffleContext context;
        int eventId;

        public bool ApplyRules { get; set; }

        public List<UserTickets> UserTicketsList { get; set; }

        public DrawWinners(int eventId, CapRaffleContext context)
        {
            this.context = context;
            this.eventId = eventId;
            ApplyRules = true;
            UserTicketsList = new List<UserTickets>();
        }

        public void ExecuteDraw()
        {
            PopulateUserRaffleTicketsList();
            ApplyRulesForEvent();
            List<UserEvent> raffle = GenerateRaffleTickets();

            Random randomGenerator = new Random();
            int availableSpots = NumberOfSpotsLeftForEvent();
            raffle.RemoveAll(x => (!x.AcceptLessSpots && x.NumberOfSpots > availableSpots));

            while (availableSpots > 0 && raffle.Count > 0)
            {
                int winnerNumber = randomGenerator.Next(raffle.Count());
                UserEvent drawnParticipant = raffle.ElementAt(winnerNumber);

                int numberOfSpotsGiven = CalculateNumberOfSpotsToGive(availableSpots, drawnParticipant.NumberOfSpots);

                raffle.RemoveAll(x => x.UserEmail == drawnParticipant.UserEmail);

                SaveWinner(drawnParticipant, numberOfSpotsGiven);
                DeleteParticipant(drawnParticipant);
                availableSpots -= numberOfSpotsGiven;
                raffle.RemoveAll(x => (!x.AcceptLessSpots && x.NumberOfSpots > availableSpots));
            }
        }

        private void PopulateUserRaffleTicketsList()
        {
            if (UserTicketsList.Count() == 0)
            {
                List<UserEvent> eventParticipants = EventParticipantsForEvent().ToList<UserEvent>();
                foreach (UserEvent ue in eventParticipants)
                {
                    UserTickets urt = new UserTickets(ue.UserEmail, 100);
                    UserTicketsList.Add(urt);
                }
            }
        }

        private void ApplyRulesForEvent()
        {
            if (ApplyRules)
            {
                var repository = new DrawingRepository();
                List<RuleParameter> rules = repository.GetRulesForEvent(eventId);
                foreach (RuleParameter rp in rules)
                {
                    InvokeRuleMethod(rp);
                }
            }
        }

        private List<UserEvent> GenerateRaffleTickets()
        {
            List<UserEvent> raffle = new List<UserEvent>();
            foreach (UserTickets ut in UserTicketsList)
            {
                UserEvent userEvent = context.UserEvents.FirstOrDefault(x => x.UserEmail.Equals(ut.Email) && x.EventId == eventId);
                for (int i = 0; i < ut.NumberOfTickets; i++)
                {
                    raffle.Add(userEvent);
                }
            }
            raffle.Shuffle();
            return raffle;
        }

        private int NumberOfSpotsLeftForEvent()
        {
            int eventAvailableSpots = context.Events.FirstOrDefault(x => x.EventId == eventId).AvailableSpots;
            List<Winner> winners = context.Winners.Where(x => x.EventId == eventId).ToList<Winner>();

            int spotsAlreadyWon = 0;
            foreach (Winner w in winners)
            {
                spotsAlreadyWon += w.NumberOfSpotsWon;
            }

            int actualAvailableSpots = eventAvailableSpots - spotsAlreadyWon;
            return actualAvailableSpots;
        }

        private int CalculateNumberOfSpotsToGive(int spotsLeft, int wantedSpots)
        {
            int spotsToGive = 0;
            if (wantedSpots > spotsLeft) spotsToGive = spotsLeft;
            if (wantedSpots <= spotsLeft) spotsToGive = wantedSpots;
            return spotsToGive;
        }

        private void InvokeRuleMethod(RuleParameter ruleParameter)
        {
            Assembly MyAssembly = Assembly.Load("CapRaffle.Domain");
            Type calledType = MyAssembly.GetType("CapRaffle.Domain.Draw." + ruleParameter.Rule.ClassName);
            if (calledType != null)
            {
                object MyObj = Activator.CreateInstance(calledType, eventId);
                calledType.InvokeMember(
                    ruleParameter.Rule.MethodName,
                    BindingFlags.InvokeMethod | BindingFlags.Default,
                    null,
                    MyObj,
                    new Object[] { UserTicketsList, ruleParameter.Param });
            }
        }


        private void SaveWinner(UserEvent drawnParticipant, int numberOfSpotsGiven)
        {
            Winner winner = new Winner
            {
                EventId = drawnParticipant.EventId,
                UserEmail = drawnParticipant.UserEmail,
                Date = DateTime.Now,
                NumberOfSpotsWon = numberOfSpotsGiven,
                CatogoryId = context.Events.FirstOrDefault(x => x.EventId == eventId).CategoryId
            };
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

        private IQueryable<UserEvent> EventParticipantsForEvent()
        {
            return context.UserEvents.Where(w => w.EventId == eventId).AsQueryable<UserEvent>();
        }
    }
}
