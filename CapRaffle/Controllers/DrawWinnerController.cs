using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.Models;

namespace CapRaffle.Controllers
{
    [Authorize]
    public class DrawWinnerController : Controller
    {
        private IEventRepository eventRepository;
        private ICategoryRepository categoryRepository;
        
        private static Random randomGenerator = new Random();

        public DrawWinnerController(IEventRepository eventRepo, ICategoryRepository categoryRepo)
        {
            eventRepository = eventRepo;
            categoryRepository = categoryRepo;
        }

        [HttpPost]
        public PartialViewResult DrawWinner(int eventId, string view)
        {
            var model = PerformDrawing(new List<UserEvent>(eventRepository.EventParticipants(eventId)), eventId);

            return PartialView(view, model);
        }
        
        private DrawWinnerViewModel PerformDrawing(List<UserEvent> eventParticipants, int eventId)
        {
            DrawWinnerViewModel viewModel = new DrawWinnerViewModel
            {
                Winners = new List<Winner>(),
                NumberOfSpotsLeft = eventRepository.Events.FirstOrDefault(x => x.EventId == eventId).AvailableSpots
            };

            int catId = eventRepository.Events.FirstOrDefault(x => x.EventId == eventId).CategoryId;
            List<UserEvent> raffleTickets = GenerateRaffleTicketsList(eventParticipants, catId);
         
            while (viewModel.NumberOfSpotsLeft > 0)
            {
                int winnerNumber = randomGenerator.Next(raffleTickets.Count());
                UserEvent drawnParticipant = raffleTickets.ElementAt(winnerNumber);
                
                int numberOfSpotsGiven = CalculateNumberOfSpotsToGive(
                    viewModel.NumberOfSpotsLeft, 
                    drawnParticipant.NumberOfSpots
                    );                

                Winner winner = new Winner
                {
                    EventId = drawnParticipant.EventId,
                    UserEmail = drawnParticipant.UserEmail,
                    Date = DateTime.Now,
                    NumberOfSpotsWon = numberOfSpotsGiven,
                    CatogoryId = catId                     
                };

                raffleTickets.RemoveAt(winnerNumber); // TODO: remove all the users raffletickets

                raffleTickets.RemoveAll(x => x.UserEmail == winner.UserEmail);
 

                eventRepository.SaveWinner(winner);
                
                viewModel.Winners.Add(winner);
                viewModel.NumberOfSpotsLeft -= winner.NumberOfSpotsWon;              
            }

            return viewModel;
        }

        private int CalculateNumberOfSpotsToGive(int spotsLeft, int wantedSpots)
        {
 	        int spotsToGive = 0;
            if (wantedSpots > spotsLeft) spotsToGive = spotsLeft;
            if (wantedSpots <= spotsLeft) spotsToGive = wantedSpots;
            return spotsToGive;
        }

        private List<UserEvent> GenerateRaffleTicketsList(List<UserEvent> eventParticipants, int categoryId) 
        {
            List<UserEvent> raffleTicketsList = new List<UserEvent>();

            foreach (UserEvent ue in eventParticipants)
            {
                int previousWins = categoryRepository.PreviousWinsInCategoryByUser(categoryId, ue.UserEmail);
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



    

    //Not in use yet.
    public class DrawingRules
    {
        public static int MaxNumberOfSpotsPerPerson;
        public static bool GiveLessSpotsThanWantedIfWinner;        
    }
}
