using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Implementation
{
    public class DrawingRepository : IDrawingRepository
    {
        private CapRaffleContext context = new CapRaffleContext();

        public IQueryable<Winner> Winners { get { return context.Winners; } }

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
            List<UserEvent> raffleTickets = GenerateRaffleTicketsList(eventId);
            Random randomGenerator = new Random();
            int availableSpots = NumberOfSpotsLeftForEvent(eventId);
            while (availableSpots > 0 && raffleTickets.Count > 0)
            {
                int winnerNumber = randomGenerator.Next(raffleTickets.Count());
                UserEvent drawnParticipant = raffleTickets.ElementAt(winnerNumber);

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

                raffleTickets.RemoveAll(x => x.UserEmail == winner.UserEmail);
                
                SaveWinner(winner);
                DeleteParticipant(drawnParticipant);
                availableSpots -= winner.NumberOfSpotsWon;
            }
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

        private int CategoryIdForEvent(int eventId)
        {
            return context.Events.FirstOrDefault(x => x.EventId == eventId).CategoryId;
        }

        private int PreviousWinsInCategoryByUser(int categoryId, string email)
        {
            return context.Winners.Where(x => x.CatogoryId == categoryId && x.UserEmail == email).Count();
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

        private List<UserEvent> GenerateRaffleTicketsList(int eventId)
        {
            List<UserEvent> eventParticipants = EventParticipantsForEvent(eventId).ToList<UserEvent>();
            List<UserEvent> raffleTicketsList = new List<UserEvent>();

            foreach (UserEvent ue in eventParticipants)
            {
                int previousWins = PreviousWinsInCategoryByUser(CategoryIdForEvent(eventId), ue.UserEmail);
                int raffleTickets = 10 - previousWins;
                if (raffleTickets < 1) raffleTickets = 1;

                for (int i = 0; i <= raffleTickets; i++)
                {
                    raffleTicketsList.Add(ue);
                }
            }
            return raffleTicketsList;
        }
    }
}
