using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Implementation
{
    public class EFDbEventRepository : IEventRepository
    {

        CapRaffleContext context = new CapRaffleContext();

        public IQueryable<Event> Events { get { return context.Events; } }   

        public void DeleteEvent(Event deleteEvent)
        {
            context.Events.DeleteObject(deleteEvent);
            context.SaveChanges();
        }

        public void SaveEvent(Event changedEvent)
        {
            if (changedEvent.EventId == 0)
            {
                context.AddToEvents(changedEvent);
            }
            context.SaveChanges();
        }
    }
}
