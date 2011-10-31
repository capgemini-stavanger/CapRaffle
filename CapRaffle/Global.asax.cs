using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CapRaffle.Infrastructure;
using CapRaffle.Controllers;

namespace CapRaffle
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Event", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Should implementing logging of exception here
            //var exception = server.GetLastError();

            Response.Clear();
            var routedata = new RouteData();
            routedata.Values.Add("controller", "Home");
            routedata.Values.Add("action", "Error");
            Server.ClearError();
            Response.TrySkipIisCustomErrors = true;
            IController home = new HomeController();
            home.Execute(new RequestContext(new HttpContextWrapper(Context), routedata));
        }
    }
}