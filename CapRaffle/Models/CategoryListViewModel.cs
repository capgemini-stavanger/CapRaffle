using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapRaffle.Models
{
    public class CategoryListViewModel
    {
        public IEnumerable<CategoryViewModel> Categories { get; set; }

    }
}