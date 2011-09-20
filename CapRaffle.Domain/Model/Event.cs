using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CapRaffle.Domain.Model
{
    [MetadataType(typeof(EventMetaData))]
    public partial class Event
    {

        public class EventMetaData
        {
            [HiddenInput(DisplayValue = false)]
            public int EventId { get; set; }

            [Required(ErrorMessage = "Please enter a name for the event")]

            public string Name { get; set; }

            [DataType(DataType.MultilineText)]
            [StringLength(300, ErrorMessage = "Description can not be more than 300 characters")]
            public string Description { get; set; }

            [Required(ErrorMessage = "Please enter number of available spots on the event")]
            [Display(Name = "Available spots")]
            [Range(1, int.MaxValue)]
            public int AvailableSpots { get; set; }

            [Required(ErrorMessage = "Please choose a category for this event")]
            [Display(Name = "Category")]
            public int CategoryId { get; set; }

            [Display(Name = "Url to information about the event")]
            public string InformationUrl { get; set; }

            [Required(ErrorMessage = "Please enter a deadline for rsvp on this event")]
            public DateTime DeadLine { get; set; }
        }
    }

    
}
