using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;

namespace CapRaffle.Controllers
{
    public class ParticipantController : Controller
    {
        private IEventRepository repository;

        public ParticipantController(IEventRepository eventRepository)
        {
            repository = eventRepository;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult Create(UserEvent participant)
        {
            participant.UserEmail = HttpContext.User.Identity.Name;
            repository.SaveParticipant(participant);
            return this.Json(true);
        }

    }
}
