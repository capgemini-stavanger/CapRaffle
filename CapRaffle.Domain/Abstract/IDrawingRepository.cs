using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Abstract
{
    public interface IDrawingRepository
    {
        IQueryable<Winner> Winners { get; }
        IQueryable<Winner> WinnersForEvent(int eventId);
        
        IQueryable<UserEvent> EventParticipantsForEvent(int eventId);

        void PerformDrawing(int eventId);

        int NumberOfSpotsLeftForEvent(int eventId);
    }
}
