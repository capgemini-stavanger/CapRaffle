using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using System.Data;
using System.Collections.Specialized;
using System.Data.Objects;
using System.Reflection;
using System.Data.Objects.DataClasses;
using System.Data.Entity;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace CapRaffle.Domain.Implementation
{
    public class EFDbEventRepository : IEventRepository
    {

        CapRaffleContext context = new CapRaffleContext();

        public IQueryable<Event> Events { get { return context.Events; } }   

        public void DeleteEvent(Event selectedEvent)
        {
            context.Events.DeleteObject(selectedEvent);
            context.SaveChanges();
        }

        public void SaveEvent(Event changedEvent)
        {
            if (changedEvent.EventId == 0)
            {
                context.AddToEvents(changedEvent);
            }
            else
            {
                context.UpdateDetachedEntity<Event>(changedEvent, x => x.EventId);
            }
            context.SaveChanges();
        }
    }
}
