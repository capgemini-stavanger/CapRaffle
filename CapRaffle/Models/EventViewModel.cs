using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapRaffle.Domain.Model;
using System.Web.Mvc;

namespace CapRaffle.Models
{
    public class EventViewModel
    {

        public Event SelectedEvent { get; set; }

        //Dropdownlist in create and edit views
        public IEnumerable<SelectListItem> Categories { get; set; }

        public IEnumerable<SelectListItem> RaffleTypes { get; set; }

        public bool UserIsParticipant { get; set; }

        public UserEvent LoggedInParticipant { get; set; }

        public IEnumerable<SelectListItem> NumberOfSpots { get; set; }
    }
}