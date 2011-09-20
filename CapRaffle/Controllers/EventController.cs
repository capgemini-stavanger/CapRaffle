﻿using System;
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
        private IEventRepository eventRepository;
        private ICategoryRepository categoryRepository;

        public EventController(IEventRepository eventRepository, ICategoryRepository categoryRepository)
        {
            this.eventRepository = eventRepository;
            this.categoryRepository = categoryRepository;
        }


        public ActionResult Index()
        {
            var model = new EventsListViewModel { Events = eventRepository.Events.OrderByDescending(x => x.EventId) };
            return View(model);
        }

        [Authorize]
        public ActionResult Create()
        {
            var newevent = new Event();
            //set proposed deadline to next full hour
            var currentDatetime = DateTime.Now;
            newevent.DeadLine = new DateTime(currentDatetime.Year, currentDatetime.Month, currentDatetime.Day, currentDatetime.Hour, 0, 0).AddHours(1);

            IEnumerable<SelectListItem> categories = categoryRepository.Categories.ToList().Select( x => 
                new SelectListItem { Text = x.Name, Value = x.CategoryId.ToString() }
                );

            categories.FirstOrDefault().Selected = true;

            var model = new EventViewModel { SelectedEvent = newevent, Categories = categories };

            
            ViewBag.action = "Create";
            return View("EventForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(EventViewModel model)
        {
            
            model.SelectedEvent.Created = DateTime.Now;
            model.SelectedEvent.Creator = HttpContext.User.Identity.Name;
            if (ModelState.IsValid)
            {
                eventRepository.SaveEvent(model.SelectedEvent);
                return RedirectToAction("Index");
            }
            return View("EventForm", model);
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
