using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapRaffle.Domain.Model;

namespace CapRaffle.Models
{
    public class EventsListViewModel
    {
        public IEnumerable<Event> Events { get; set; }

    }
}