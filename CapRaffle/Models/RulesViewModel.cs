﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapRaffle.Domain.Model;
using CapRaffle.Domain.Raffle;

namespace CapRaffle.Models
{
    public class RulesViewModel
    {

        public List<Rule> AvailableRules { get; set; }
        public List<RuleParameter> RulesForEvent { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
    }
}