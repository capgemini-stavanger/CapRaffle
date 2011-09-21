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

            var model = new EventViewModel { SelectedEvent = newevent, Categories = categorySelectList() };
            
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
                this.Success("The event has been created.");
                return RedirectToAction("Index");
            }

            
            model.Categories = categorySelectList();
            return View("EventForm", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var selectedEvent = eventRepository.Events.Where(x => x.EventId == id).FirstOrDefault();
            if (selectedEvent.Creator.Equals(HttpContext.User.Identity.Name))
            {
                eventRepository.DeleteEvent(selectedEvent);
                this.Success("The Event has been deleted.");
                return RedirectToAction("Index");
            }
            else
            {
                this.Error("You can only delete events you created.");
                return RedirectToAction("Details", new { id = id });
            }
            
        }

        public ActionResult Details(int id)
        {
            var model = new EventViewModel { SelectedEvent = eventRepository.Events.Where(x => x.EventId == id).FirstOrDefault() };
            return View(model);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var selectedEvent = eventRepository.Events.Where(x => x.EventId == id).FirstOrDefault();
            if (!selectedEvent.Creator.Equals(HttpContext.User.Identity.Name))
            {
                this.Info("You can only edit your own events.");
                return RedirectToAction("Details", new { id = id });
            }

            var model = new EventViewModel { SelectedEvent = selectedEvent, Categories = categorySelectList() };

            return View("EventForm", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public ActionResult Edit(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                eventRepository.SaveEvent(model.SelectedEvent);
                this.Success("Changes on event has been saved");
                return RedirectToAction("Details", new { id = model.SelectedEvent.EventId });
            }

            model.Categories = categorySelectList();
            return View("EventForm", model);
        }



        private IEnumerable<SelectListItem> categorySelectList() {
            var categories = categoryRepository.Categories.ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.CategoryId.ToString() });
            categories.FirstOrDefault().Selected = true;
            return categories;
        }
    }
}
