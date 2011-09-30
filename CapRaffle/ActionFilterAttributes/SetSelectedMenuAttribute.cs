using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapRaffle.ActionFilterAttributes
{
    public class SetSelectedMenuAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var controller = filterContext.RouteData.Values["controller"];
            var action = filterContext.RouteData.Values["action"];

            var viewBag = filterContext.Controller.ViewBag;
            viewBag.MenuController = controller;
            viewBag.MenuAction = action;
        }
    }
}