﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Abstract
{
    public interface IEventRepository
    {
        IQueryable<Event> Events { get; }

        void DeleteEvent(int id);

        void SaveEvent(Event changedEvent);
    }
}