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
        private IAccountRepository accountrepository;

        public ParticipantController(IEventRepository eventRepository, IAccountRepository accountrepository)
        {
            repository = eventRepository;
            this.accountrepository = accountrepository;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult Create(UserEvent participant)
        {
            if (repository.Users.Where(x => x.Email.Equals(participant.UserEmail)).Count() == 0)
            {
                var name = participant.UserEmail;
                if(name.Contains(".") || name.Contains("-") || name.Contains(".")) 
                {
                    name = participant.UserEmail.Replace("capgemini.com", "");
                    name = name.Replace("-", " ");
                    name = name.Replace(".", " ");
                    name = name.Substring(0, name.Length - 1);
                }
                accountrepository.Create(participant.UserEmail, null, name);
            }

            repository.SaveParticipant(participant);
            return this.Json(true);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(UserEvent participant)
        {
            if (!HttpContext.User.Identity.Name.Equals(participant.UserEmail))
                return this.Json(false);

            repository.DeleteParticipant(participant);
            return this.Json(true);
        }


        [HttpPost]
        [Authorize]
        public JsonResult GetUsers(string email)
        {
            return this.Json(repository.Users.Where(x => x.Email.StartsWith(email)).Select(x => x.Email).ToList());
        }
    }
}
