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
        public PartialViewResult DrawWinner(int eventId)
        {
            var model = PerformDrawing(new List<UserEvent>(eventRepository.EventParticipants(eventId)), eventId);

            return PartialView(model);
        }
        
        private DrawWinnerViewModel PerformDrawing(List<UserEvent> eventParticipants, int eventId)
        {
            DrawWinnerViewModel viewModel = new DrawWinnerViewModel
            {
                Winners = new List<Winner>(),
                NumberOfSpotsLeft = eventRepository.Events.FirstOrDefault(x => x.EventId == eventId).AvailableSpots
            };            

            while (viewModel.NumberOfSpotsLeft > 0)
            {
                int winnerNumber = randomGenerator.Next(eventParticipants.Count());
                UserEvent drawnParticipant = eventParticipants.ToList().ElementAt(winnerNumber);
                
                int numberOfSpotsGiven = CalculateNumberOfSpotsToGive(
                    viewModel.NumberOfSpotsLeft, 
                    drawnParticipant.NumberOfSpots
                    );                
                int categoryId = eventRepository.Events.FirstOrDefault(x => x.EventId == eventId).CategoryId;

                Winner winner = new Winner
                {
                    EventId = drawnParticipant.EventId,
                    UserEmail = drawnParticipant.UserEmail,
                    Date = DateTime.Now,
                    NumberOfSpotsWon = numberOfSpotsGiven,
                    CatogoryId = categoryId                     
                };

                eventParticipants.RemoveAt(winnerNumber);
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
    }

    //Not in use yet.
    public class DrawingRules
    {
        public static int MaxNumberOfSpotsPerPerson;
        public static bool GiveLessSpotsThanWantedIfWinner;        
    }
}
