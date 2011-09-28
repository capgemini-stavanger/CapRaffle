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
            
            if (ModelState.IsValid)
            {
                eventRepository.SaveEvent(model.SelectedEvent);
                this.Success("The event has been created.");
                return RedirectToAction("Index");
            }

            model.Categories = categorySelectList();
            ViewBag.action = "Create";
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
            var selectedEvent = eventRepository.Events.Where(x => x.EventId == id).FirstOrDefault();
            var participant = selectedEvent.UserEvents.Where(x => x.UserEmail == HttpContext.User.Identity.Name).FirstOrDefault();
            var model = new EventViewModel 
            { 
                SelectedEvent = selectedEvent, 
                UserIsParticipant = participant != null, 
                LoggedInParticipant = participant,
                NumberOfSpots = NumberofSpotsList(selectedEvent.AvailableSpots),
                RaffleTypes = getRaffleTypes()
            };
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
            ViewBag.action = "Edit";
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
            ViewBag.action = "Edit";
            return View("EventForm", model);
        }

        private IEnumerable<SelectListItem> NumberofSpotsList(int numberOfSpots)
        {
            var selectedItemList = new List<SelectListItem>();
            for (int i = 1; i <= numberOfSpots; i++)
            {
                var selected = i == 1 ? true : false;
                selectedItemList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = selected });
                
            }
            return selectedItemList;
        }

        private IEnumerable<SelectListItem> categorySelectList() {
            var categories = categoryRepository.Categories.ToList().Where(x => x.IsActive).Select(x => new SelectListItem { Text = x.Name, Value = x.CategoryId.ToString() });
            categories.FirstOrDefault().Selected = true;
            return categories;
        }

        private IEnumerable<SelectListItem> getRaffleTypes()
        {
            var raffleTypes = new List<SelectListItem>();
            raffleTypes.Add(new SelectListItem { Text = "Default", Value="_DrawWinner", Selected = true });
            raffleTypes.Add(new SelectListItem { Text = "FlightBoard", Value = "_DrawFlightBoard" });
            raffleTypes.Add(new SelectListItem { Text = "FlipBox", Value = "_DrawFlipBox" });
            return raffleTypes;
        }
    }
}
