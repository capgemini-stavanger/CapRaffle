using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapRaffle.Models
{
    public class MenuViewModel
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Text { get; set; }
        public bool isSelected { get; set; }
    }
}