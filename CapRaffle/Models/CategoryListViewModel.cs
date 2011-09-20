using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapRaffle.Domain.Model;

namespace CapRaffle.Models
{
    public class CategoryListViewModel
    {
        public IEnumerable<Category> Categories { get; set; }

    }
}