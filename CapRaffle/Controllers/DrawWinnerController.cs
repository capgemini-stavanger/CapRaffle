using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;

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
        public ActionResult Index(int eventId)
        {
            IQueryable<UserEvent> eventParticipants = eventRepository.EventParticipants(eventId);

            return View(eventParticipants);
        }

        public PartialViewResult DrawWinner(IQueryable<UserEvent> eventParticipants)
        {
            int winnerNumber = randomGenerator.Next(eventParticipants.Count());
            UserEvent drawnParticipant = eventParticipants.ElementAt(winnerNumber);
            Winner winner = new Winner 
                {
                    EventId = drawnParticipant.EventId,
                    UserEmail = drawnParticipant.UserEmail,
                    Date = DateTime.Now,
                    NumberOfSpotsWon = drawnParticipant.NumberOfSpots
                };
            eventRepository.SaveWinner(winner);
            return PartialView(winner);
        }
    }
}
