using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapRaffle.Models
{
    public class CategoryViewModel
    {
        [HiddenInput(DisplayValue=false)]
        public int CategoryId { get; set; }

        public string Name { get; set; }

    }
}