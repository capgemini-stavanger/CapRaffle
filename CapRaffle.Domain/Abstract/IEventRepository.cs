﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;
using CapRaffle.Domain.Raffle;

namespace CapRaffle.Domain.Abstract
{
    public interface IEventRepository
    {
        IQueryable<Event> Events { get; }

        IQueryable<User> Users { get; }

        IQueryable<UserEvent> Participants { get; }

        IQueryable<UserEvent> EventParticipants(int eventId);

        IQueryable<Category> Categories { get; }

        IQueryable<Rule> AvailableRules { get; }

        void SaveCategory(Category category);

        void DeleteEvent(Event selectedEvent);

        void SaveEvent(Event changedEvent);

        void DeleteParticipant(UserEvent participant);

        void SaveParticipant(UserEvent participant);

        void SaveRulesForCategory(int categoryId, List<RuleParameter> ruleParameters);

        List<RuleParameter> GetRulesForCategory(int categoryId);
    }
}
