using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Rules
{
    /*
     * Rules to be implemented
     * 
     * If a user have never won in this category before, give him the event tickets, then draw the rest.
     * Reduce chance of winning by XX% for each previous win. (10,25,50,75%)
     * When RaffleTickets are taken away from a user, devide them between the user(s) with least wins in the category.
     * When RaffleTickets are taken away from a user, devide them between users with no wins in the category.
     * Max RaffleTickets per user is: xx (?)
     * Min RaffleTickets per user is: xx (?)
     * 
     */

    public class StandardRules
    {
        
        private CapRaffleContext context = new CapRaffleContext();
        private int categoryId;

        public StandardRules(int categoryId)
        {
            this.categoryId = categoryId;
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
