using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Abstract
{
    public interface IEventRepository
    {
        IQueryable<Event> Events { get; }

        void DeleteEvent(Event selectedEvent);

        void SaveEvent(Event changedEvent);

        void SaveParticipant(UserEvent participant);

        void DeleteParticipant(UserEvent participant);
    }
}
