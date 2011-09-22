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

        public DrawWinnerController(IEventRepository eventRepo, ICategoryRepository categoryRepo)
        {
            eventRepository = eventRepo;
            categoryRepository = categoryRepo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(int eventId)
        {
            IQueryable<UserEvent> eventParticipants = eventRepository.EventParticipants(eventId);

            return View(eventParticipants);
        }
    }
}
