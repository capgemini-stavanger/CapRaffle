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

        IQueryable<UserEvent> Participants { get; }

        IQueryable<UserEvent> EventParticipants(int eventId);

        void DeleteEvent(Event selectedEvent);

        void SaveEvent(Event changedEvent);

        void DeleteParticipant(UserEvent participant);

        void SaveParticipant(UserEvent participant);

        void SaveWinner(Winner winner);
    }
}
