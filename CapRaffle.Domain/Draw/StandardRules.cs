using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Draw
{
    /*
     * Rules to be implemented
     * 
     * If a user have never won in this category before, give him the event tickets, then draw the rest.
     * When RaffleTickets are taken away from a user, devide them between the user(s) with least wins in the category.
     * When RaffleTickets are taken away from a user, devide them between users with no wins in the category.
     * Min RaffleTickets per user is: xx (?)
     * 
     */

    public class StandardRules
    {

        private CapRaffleContext context = new CapRaffleContext();
        private int categoryId;
        private int eventId;

        public StandardRules(int eventId)
        {
            this.eventId = eventId;
            categoryId = context.Events.FirstOrDefault(x => x.EventId == eventId).CategoryId;
            
        }

        public void GiveTicketsToUsersWhoNeverHasWonInThisCategory(List<UserTickets> userRaffleTicketsList, int number)
        {
            var NeverWon = new List<UserTickets>();
            foreach (var urt in userRaffleTicketsList)
            {
                if (PreviousWinsInCategoryByUser(urt.Email) == 0)
                {
                    NeverWon.Add(urt);
                }
            }
            if (NeverWon.Count() > 0)
            {
                var drawing = new DrawWinners(eventId, context);
                drawing.ApplyRules = false;
                drawing.UserTicketsList = NeverWon;
                drawing.ExecuteDraw();
                var winners = context.Winners.Where(x => x.EventId == eventId).ToList();
                winners.ForEach(x => userRaffleTicketsList.RemoveAll(y => y.Email == x.UserEmail));                
            }
        }

        public void MinimumRaffleTicketsPerUserIs(List<UserTickets> userRaffleTicketsList, int number)
        {
            foreach (UserTickets urt in userRaffleTicketsList)
            {
                if( urt.NumberOfTickets < number) 
                {
                    urt.NumberOfTickets = number;
                }
            }
        }

        public void ReduceChanceOfWinningByPercentForEachPreviousWin(List<UserTickets> userRaffleTicketsList, int percent)
        {
            foreach (UserTickets urt in userRaffleTicketsList)
            {
                int previousWins = PreviousWinsInCategoryByUser(urt.Email);
                for(int i = 0; i < previousWins; i++) 
                {
                    int currentNumberOfTickets = urt.NumberOfTickets;
                    int newNumberOfTickets = currentNumberOfTickets - ((currentNumberOfTickets * percent) / 100);
                    urt.NumberOfTickets = newNumberOfTickets;
                }
            }
        }

        private int PreviousWinsInCategoryByUser(string email)
        {
            return context.Winners.Where(x => x.CatogoryId == categoryId && x.UserEmail == email).Count();
        }
    }
}
