using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;
using CapRaffle.Domain.Rules;

namespace CapRaffle.Domain.Abstract
{
    public interface IDrawingRepository
    {
        IQueryable<Winner> Winners { get; }
        IQueryable<Winner> WinnersForEvent(int eventId);
        
        IQueryable<UserEvent> EventParticipantsForEvent(int eventId);

        IQueryable<Rule> AvailableRules { get; }

        void PerformDrawing(int eventId);
        
        void RemoveWinner(Winner winner);

        int NumberOfSpotsLeftForEvent(int eventId);

        List<RuleParameter> GetRulesForEvent(int eventId);

        void SaveRulesForEvent(int eventId, List<RuleParameter> ruleparameters);
    }
}
