using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Models;
using CapRaffle.Domain.Model;
using CapRaffle.Domain.Abstract;
using CapRaffle.ActionFilterAttributes;

namespace CapRaffle.Controllers
{
    [SetSelectedMenu]
    public class EventController : Controller
    {
        private IEventRepository eventRepository;
        public int PageSize = 12;

        public EventController(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }

        //public ActionResult Index()
        //{
        //    DateTime date = DateTime.Now.Date;
        //    var model = new EventsListViewModel { Events = eventRepository.Events.Where(x => x.DeadLine >= date).OrderByDescending(x => x.EventId) };
        //    return View(model);
        //}

        public ActionResult Index(bool archive = false, int page = 1)
        {
            DateTime date = DateTime.Now.Date.AddDays(-5);
            var model = new EventsListViewModel();
            int numberOfEvents = 0;
            if (archive)
            {
                numberOfEvents = eventRepository.Events.Where(x => x.DeadLine <= date).Count();
                model.Events = eventRepository.Events.Where(x => x.DeadLine <= date).OrderBy(x => x.Name).Skip((page - 1) * PageSize).Take(PageSize); 
                model.Archive = true;    
            }
            else
            {
                numberOfEvents = eventRepository.Events.Where(x => x.DeadLine >= date).Count();
                model.Events = eventRepository.Events.Where(x => x.DeadLine >= date).OrderBy(x => x.Name).Skip((page - 1) * PageSize).Take(PageSize);
            }
            
            PagingInfo pi = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = PageSize,
                TotalItems = numberOfEvents
            };
            model.PagingInfo = pi;
            return View(model);
        }

        [Authorize]
        public ActionResult Create()
        {
            var newevent = new Event();
            //set proposed deadline to next full hour
            var currentDatetime = DateTime.Now;
            newevent.DeadLine = new DateTime(currentDatetime.Year, currentDatetime.Month, currentDatetime.Day, currentDatetime.Hour, 0, 0).AddHours(1);
            newevent.StartTime = new DateTime(currentDatetime.Year, currentDatetime.Month, currentDatetime.Day, currentDatetime.Hour, 0, 0).AddHours(8);
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

        [Authorize]
        public ActionResult CreateBasedOnOldEvent(int id)
        {
            var selectedEvent = eventRepository.Events.Where(x => x.EventId == id).FirstOrDefault();
            var newevent = new Event();
            if (selectedEvent != null)
            {
                var currentDatetime = DateTime.Now;
                newevent.DeadLine = new DateTime(currentDatetime.Year, currentDatetime.Month, currentDatetime.Day, currentDatetime.Hour, 0, 0).AddHours(1);
                newevent.StartTime = new DateTime(currentDatetime.Year, currentDatetime.Month, currentDatetime.Day, currentDatetime.Hour, 0, 0).AddHours(8);
                newevent.Name = selectedEvent.Name;
                newevent.AvailableSpots = selectedEvent.AvailableSpots;
                newevent.InformationUrl = selectedEvent.InformationUrl;
                newevent.Description = selectedEvent.Description;
                newevent.Category = selectedEvent.Category;
            }
            var model = new EventViewModel { SelectedEvent = newevent, Categories = categorySelectList() };

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

            ViewBag.MenuController = "Event";
            ViewBag.MenuAction = "Index";
            ViewBag.isCreator = selectedEvent.Creator.Equals(HttpContext.User.Identity.Name);
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
            var categories = eventRepository.Categories.ToList().Where(x => x.IsActive).Select(x => new SelectListItem { Text = x.Name, Value = x.CategoryId.ToString() });
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
