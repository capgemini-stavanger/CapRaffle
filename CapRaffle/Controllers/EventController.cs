using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Model;
using CapRaffle.Domain.Abstract;

namespace CapRaffle.Controllers
{
    public class EventController : Controller
    {
        private IEventRepository repository;

        public EventController(IEventRepository repository)
        {
            this.repository = repository;
        }


        public ActionResult Index()
        {
            return View(repository.Events);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Event newEvent)
        {
            if (ModelState.IsValid)
            {
                repository.SaveEvent(newEvent);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
