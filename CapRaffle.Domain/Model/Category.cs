using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CapRaffle.Domain.Model
{
    [MetadataType(typeof(CategoryMetaData))]
    public partial class Category
    {
        public class CategoryMetaData
        {
            [HiddenInput(DisplayValue = false)]
            public int CategoryId { get; set; }

            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }
        }
    }
}
