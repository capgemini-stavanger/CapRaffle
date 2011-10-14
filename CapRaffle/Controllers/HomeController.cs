using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.ActionFilterAttributes;

namespace CapRaffle.Controllers
{
    [SetSelectedMenu]
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult About()
        {
            return View();
        }

    }
}
