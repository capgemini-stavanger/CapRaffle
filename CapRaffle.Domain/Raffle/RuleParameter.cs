using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Raffle
{
    public class RuleParameter
    {
        public Rule Rule { get; set; }
        public int Param { get; set; }
    }
}
