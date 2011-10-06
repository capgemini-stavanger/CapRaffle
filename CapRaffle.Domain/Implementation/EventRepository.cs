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

namespace CapRaffle.Domain.Implementation
{
    public class EventRepository : IEventRepository
    {

        CapRaffleContext context = new CapRaffleContext();

        public IQueryable<Event> Events { get { return context.Events; } }

        public IQueryable<UserEvent> Participants { get { return context.UserEvents; } }

        public IQueryable<User> Users { get { return context.Users; } }

        public IQueryable<UserEvent> EventParticipants(int eventId)
        {
            return context.UserEvents.Where(x => x.EventId == eventId).AsQueryable();
        }

        public IQueryable<Category> Categories
        {
            get { return context.Categories; }
        }

        public void SaveCategory(Category category)
        {
            if (category.CategoryId == 0)
            {
                context.AddToCategories(category);
            }
            else
            {
                try
                {
                    context.UpdateDetachedEntity<Category>(category, x => x.CategoryId);
                }
                catch (Exception)
                {
                    context.ApplyCurrentValues<Category>(category.EntityKey.EntitySetName, category);
                }
            }
            context.SaveChanges();
        }
        
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

        public void DeleteParticipant(UserEvent participant)
        {
            context.UpdateDetachedEntity<UserEvent>(participant, x => x.EventId);
            context.UserEvents.DeleteObject(participant);
            context.SaveChanges();
        }

        public void SaveParticipant(UserEvent participant)
        {
            if (context.UserEvents.Where(x => x.EventId == participant.EventId && x.UserEmail.Equals(participant.UserEmail)).Count() == 0)
            {
                context.AddToUserEvents(participant);
            }
            else
            {
                context.UpdateDetachedEntity<UserEvent>(participant, x => x.EventId);
            }
            context.SaveChanges();
        }
    }
}
