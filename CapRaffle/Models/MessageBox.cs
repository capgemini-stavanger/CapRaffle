using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapRaffle.Models
{
    public static class Extensions
    {
        public static void Info(this Controller controller, string message)
        {
            controller.TempData["Info"] = message;
        }
        public static void Success(this Controller controller, string message)
        {
            controller.TempData["Success"] = message;
        }
        public static void Warning(this Controller controller, string message)
        {
            controller.TempData["Warning"] = message;
        }
        public static void Error(this Controller controller, string message)
        {
            controller.TempData["Error"] = message;
        }
    }
}