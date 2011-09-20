using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Models;
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
            var model = new EventsListViewModel { Events = repository.Events.OrderByDescending(x => x.EventId) };
            return View(model);
        }

        public ActionResult Create()
        {
            var newevent = new Event();

            //set proposed deadline to next full hour
            var currentDatetime = DateTime.Now;
            newevent.DeadLine = new DateTime(currentDatetime.Year, currentDatetime.Month, currentDatetime.Day, currentDatetime.Hour, 0, 0).AddHours(1);
            ViewBag.action = "Create";
            return View("EventForm", newevent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event newEvent)
        {
            newEvent.Created = DateTime.Now;
            newEvent.Creator = "NotImplementedLogin";
            if (ModelState.IsValid)
            {
                repository.SaveEvent(newEvent);
                return RedirectToAction("Index");
            }
            return View("EventForm");
        }

        public ActionResult Edit(int id)
        {


            return View();
        }

        [HttpPost]
        public ActionResult Edit(Event changedEvent)
        {
            return View();
        }
    }
}
