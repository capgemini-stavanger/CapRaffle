using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.ActionFilterAttributes;

namespace CapRaffle.Controllers
{
    [SetSelectedMenu]
    public class StatisticController : Controller
    {
        IStatisticRepository repository;

        public StatisticController(IStatisticRepository repository)
        {
            this.repository = repository;
        }


        public ActionResult Category(int categoryId)
        {
            var winners = repository.CategoryStatistics(categoryId);
            ViewBag.MenuController = "Category";
            ViewBag.MenuAction = "Index";
            return View(winners);
        }

        public ActionResult UserStatisticPartial()
        {
            var stats = repository.UserStatistics(HttpContext.User.Identity.Name);
            return PartialView("_UserStatistic", stats);
        }
    }
}
