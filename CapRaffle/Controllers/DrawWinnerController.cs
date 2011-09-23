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
        public PartialViewResult Index(int eventId)
        {
            IQueryable<UserEvent> eventParticipants = eventRepository.EventParticipants(eventId);

            return PartialView(eventParticipants);
        }

        [HttpPost]
        public PartialViewResult DrawWinner(int eventId)
        {
            IQueryable<UserEvent> eventParticipants = eventRepository.EventParticipants(eventId);

            int categoryId = eventRepository.Events.FirstOrDefault(x => x.EventId == eventId).CategoryId;

            int winnerNumber = randomGenerator.Next(eventParticipants.Count());
            UserEvent drawnParticipant = eventParticipants.ToList().ElementAt(winnerNumber);
            Winner winner = new Winner 
                {
                    EventId = drawnParticipant.EventId,
                    UserEmail = drawnParticipant.UserEmail,
                    Date = DateTime.Now,
                    NumberOfSpotsWon = drawnParticipant.NumberOfSpots,
                    CatogoryId = categoryId
                };
            eventRepository.SaveWinner(winner);

            DrawWinnerViewModel viewModel = new DrawWinnerViewModel
            {
                Winners = new List<Winner>
                {
                    winner
                }
            };

            return PartialView(viewModel);
        }
    }
}
