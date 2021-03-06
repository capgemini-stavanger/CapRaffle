﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.ActionFilterAttributes;
using CapRaffle.Models;

namespace CapRaffle.Controllers
{
    [SetSelectedMenu]
    public class ParticipantController : Controller
    {
        private IEventRepository repository;
        private IAccountRepository accountrepository;

        public ParticipantController(IEventRepository eventRepository, IAccountRepository accountrepository)
        {
            repository = eventRepository;
            this.accountrepository = accountrepository;
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(UserEvent participant)
        {
            try
            {
                if (repository.Users.Where(x => x.Email.Equals(participant.UserEmail)).Count() == 0)
                {
                    participant.UserEmail = participant.UserEmail.ToLower();
                    var name = participant.UserEmail;
                    if (name.Contains(".") || name.Contains("-") || name.Contains("."))
                    {
                        name = participant.UserEmail.Replace("capgemini.com", "");
                        name = name.Replace("-", " ");
                        name = name.Replace(".", " ");
                        name = name.Substring(0, name.Length - 1);
                        name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
                    }
                    accountrepository.Create(participant.UserEmail, null, name);
                }
                repository.SaveParticipant(participant);
            }
            catch (Exception e)
            {
                this.Error(e.Message);
            }
            return RedirectToAction("GetParticipants", new { eventId = participant.EventId });
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(UserEvent participant)
        {
            if (!HttpContext.User.Identity.Name.Equals(participant.UserEmail))
            {
                var selectedEvent = repository.Events.Where(x => x.EventId == participant.EventId).FirstOrDefault();
                if (!HttpContext.User.Identity.Name.Equals(selectedEvent.Creator))
                {
                    return this.Json(false);
                }
            }

            repository.DeleteParticipant(participant);
            return this.Json(true);
        }


        [HttpPost]
        [Authorize]
        public JsonResult GetUsers(string email)
        {
            return this.Json(repository.Users.Where(x => x.Email.StartsWith(email)).Select(x => x.Email).ToList());
        }


        public PartialViewResult GetParticipants(int eventId)
        {
            var participants = repository.Participants.Where(x => x.EventId == eventId).ToList();
            var selectEvent = repository.Events.Where(x => x.EventId == eventId).FirstOrDefault();
            ViewBag.isCreator = selectEvent.Creator.Equals(HttpContext.User.Identity.Name);
            return PartialView("_GetParticipants", participants);
        }
    }
}
