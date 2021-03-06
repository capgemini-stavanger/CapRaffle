﻿using System;
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
            var userStatistics = GetUserStatisticForCategory();

            var catStatistics = new CategoryStatistics();
            catStatistics.Category = category.FirstOrDefault();
            catStatistics.Top5Winners = userStatistics.OrderByDescending(x => x.Wins).Take(5).ToList();
            catStatistics.Top5Loosers = userStatistics.OrderByDescending(x => x.Losses).Take(5).ToList();
            catStatistics.NumberOfEventsInCategory = NumberOfEventsInCategory();
            catStatistics.NumberOfRaffleParticipantsInCategory = NumberOfRaffleParticipantsInCategory();
            catStatistics.NumberOfEventTicketsHandedOut = NumberOfEventTicketsHandedOut();
            catStatistics.UniqueNumberOfRaffleParticipantsInCategory = UniqueNumberOfRaffleParticipantsInCategory();
            catStatistics.NumberOfEventTicketsNotHandedOut = NumberOfEventTicketsNotHandedOut();
            catStatistics.NumberOfTimesEventCreatorHasWonHisOwnRaffle = NumberOfTimesEventCreatorHasWonHisOwnRaffle();

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
            var groupWinners = winners.GroupBy(x => x.User.Name).ToList();
            foreach (var winner in groupWinners)
            {
                if (statistics.Where(x => x.Name.Equals(winner.Key)).Count() == 0)
                {
                    var wins = winner.Count();
                    var numberOfParticipations = wins;
                    var losses = 0;
                    statistics.Add(
                        new UserStatistics
                        {
                            Name = winner.Key,
                            Wins = wins,
                            NumberOfParticipations = numberOfParticipations,
                            Losses = losses
                        });
                }
            }
            return statistics;
        }

        private int NumberOfEventsInCategory()
        {
            return context.Events.Where(x => x.CategoryId == categoryId).Count();
        }

        private int NumberOfRaffleParticipantsInCategory()
        {
            var participants = context.UserEvents.Where(x => x.Event.CategoryId == categoryId).Count();
            var winners = context.Winners.Where(x => x.Event.CategoryId == categoryId).Count();
            return participants + winners;
        }
        
        private int UniqueNumberOfRaffleParticipantsInCategory()
        {
            var participants = context.UserEvents.Where(x => x.Event.CategoryId == categoryId).GroupBy(x => x.UserEmail).ToList();
            var winners = context.Winners.Where(x => x.Event.CategoryId == categoryId).GroupBy(x => x.UserEmail).ToList();
            var unique = new HashSet<string>();
            participants.ForEach(x => unique.Add(x.Key));
            winners.ForEach(x => unique.Add(x.Key));
            return unique.Count;
        }

        private int NumberOfEventTicketsHandedOut()
        {
            var winners = context.Winners.Where(x => x.CatogoryId == categoryId);
            if(winners.Count() > 0)
                return winners.Sum(x => x.NumberOfSpotsWon);
            return 0;
        }

        private int NumberOfEventTicketsNotHandedOut()
        {
            var unUsedTickets = context.Events.Where(x => x.CategoryId == categoryId);
            if(unUsedTickets.Count() > 0)
                return unUsedTickets.Sum(x => x.AvailableSpots) - NumberOfEventTicketsHandedOut();
            return 0;
        }

        private int NumberOfTimesEventCreatorHasWonHisOwnRaffle()
        {
            var events = context.Events.Where(x => x.CategoryId == categoryId).ToList();
            int number = 0;
            foreach (var selectedEvent in events)
            {
                number += selectedEvent.Winners
                    .Where(x => x.UserEmail.Equals(selectedEvent.Creator)).Count() > 0 ? 1 : 0;
            }
            return number;
        }



        public UserStatistics UserStatistics(string email)
        {
            User user = (User)context.Users.Where(x => x.Email == email).FirstOrDefault();
            UserStatistics us = new UserStatistics();
            if (user != null)
            {
                us.Wins = context.Winners.Where(x => x.UserEmail == email).Count();
                us.Losses = context.UserEvents.Where(x => x.UserEmail == email).Count();
                us.NumberOfParticipations = us.Wins + us.Losses;
                us.TotalSpots = NumberOfUserTicketsHandedOut(email);
            }
            else { throw new ArgumentException("User does not exist"); }
            return us;
        }

        private int NumberOfUserTicketsHandedOut(string email)
        {
            var winners = context.Winners.Where(x => x.UserEmail == email);
            if (winners.Count() > 0)
                return winners.Sum(x => x.NumberOfSpotsWon);
            return 0;
        }
    }
}
