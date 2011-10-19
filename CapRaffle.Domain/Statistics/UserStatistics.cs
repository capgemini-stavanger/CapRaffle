using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapRaffle.Domain.Statistics
{
    public class UserStatistics
    {
        public string Name { get; set; }
        public int Wins { get; set; }
        public int NumberOfParticipations { get; set; }
        public int Losses { get; set; }
        public int TotalSpots { get; set; }
    }
}
