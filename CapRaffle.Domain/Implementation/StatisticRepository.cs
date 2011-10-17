using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.Domain.Statistics;

namespace CapRaffle.Domain.Implementation
{

    public class StatisticRepository : IStatisticRepository
    {
        CapRaffleContext context = new CapRaffleContext();
        private int categoryId;

        public CategoryStatistics CategoryStatistics(int categoryId)
        {
            this.categoryId = categoryId;
            var category = context.Categories.Where(x => x.CategoryId == categoryId);
            if (category.Count() == 0)
            {
                throw new ArgumentException("Category does not exist");
            }
            var catStatistics = new CategoryStatistics();
            catStatistics.Category = category.FirstOrDefault();
            catStatistics.Top5Winners = GetUserStatisticForCategory().OrderByDescending(x => x.Wins).Take(5).ToList();
            catStatistics.Top5Loosers = GetUserStatisticForCategory().OrderByDescending(x => x.Losses).Take(5).ToList();
            catStatistics.NumberOfEventsInCategory = NumberOfEventsInCategory();
            catStatistics.NumberOfRaffleParticipantsInCategory = NumberOfRaffleParticipantsInCategory();
            catStatistics.NumberOfEventTicketsHandedOut = NumberOfEventTicketsHandedOut();
            catStatistics.UniqueNumberOfRaffleParticipantsInCategory = UniqueNumberOfRaffleParticipantsInCategory();
            catStatistics.NumberOfEventTicketsNotHandedOut = NumberOfEventTicketsNotHandedOut();

            return catStatistics;
        }

        private List<UserStatistics> GetUserStatisticForCategory()
        {
            var participants = context.UserEvents.Where(x => x.Event.CategoryId == categoryId).GroupBy(x => x.User.Name).ToList();
            var winners = context.Winners.Where(x => x.CatogoryId == categoryId).ToList();
            var statistics = new List<UserStatistics>();

            foreach (var participant in participants)
            {
                var wins = winners.Where(x => x.User.Name.Equals(participant.Key)).Count();
                var numberOfParticipations = participant.Count() + wins;
                var losses = numberOfParticipations - wins;
                statistics.Add(
                    new UserStatistics { 
                        Name = participant.Key, 
                        Wins = wins, 
                        NumberOfParticipations = numberOfParticipations, 
                        Losses = losses 
                    });
            }
            return statistics;
        }

        private int NumberOfEventsInCategory()
        {
            return context.Events.Where(x => x.CategoryId == categoryId).Count();
        }

        private int NumberOfRaffleParticipantsInCategory()
        {
            return context.UserEvents.Where(x => x.Event.CategoryId == categoryId).Count();
        }
        
        private int UniqueNumberOfRaffleParticipantsInCategory()
        {
            return context.UserEvents.Where(x => x.Event.CategoryId == categoryId).GroupBy(x => x.UserEmail).Count();
        }

        private int NumberOfEventTicketsHandedOut()
        {
            return context.Winners.Where(x => x.CatogoryId == categoryId).Sum(x => x.NumberOfSpotsWon);
        }

        private int NumberOfEventTicketsNotHandedOut()
        {
            return context.Events.Where(x => x.CategoryId == categoryId).Sum(x => x.AvailableSpots) - NumberOfEventTicketsHandedOut();
        }

        public int NumberOfTimesEventCreatorHasWonHisOwnRaffle()
        {
            throw new NotImplementedException();
        }
    }
}
