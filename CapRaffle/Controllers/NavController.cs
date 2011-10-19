using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Models;

namespace CapRaffle.Controllers
{
    public class NavController : Controller
    {
        //
        // GET: /Nav/

        public PartialViewResult Menu(string controller, string action)
        {
            var menuViewModel = new List<MenuViewModel>();
            menuViewModel.Add(new MenuViewModel { Text = "Event list", Action = "index", Controller = "event" });
            menuViewModel.Add(new MenuViewModel { Text = "Create new event", Action = "create", Controller = "event" });
            menuViewModel.Add(new MenuViewModel { Text = "Category list", Action = "index", Controller = "category" });
            menuViewModel.Add(new MenuViewModel { Text = "Create new category", Action = "create", Controller = "category" });

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                menuViewModel.Add(new MenuViewModel { Text = "Log in", Action = "LogOn", Controller = "Account" });
                menuViewModel.Add(new MenuViewModel { Text = "Register new account", Action = "Register", Controller = "Account" });
            }

            menuViewModel.Add(new MenuViewModel { Text = "About CapRaffle", Action = "About", Controller = "Home" });
            var selected = menuViewModel.Where(x => x.Controller.ToLower().Equals(controller.ToLower()) && x.Action.ToLower().Equals(action.ToLower())).FirstOrDefault();
            if(selected != null) selected.isSelected = true;

            return PartialView(menuViewModel);
        }
    }
}
