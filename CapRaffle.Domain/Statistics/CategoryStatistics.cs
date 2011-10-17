using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Statistics
{
    public class CategoryStatistics
    {
        public Category Category { get; set; }
        public List<UserStatistics> Top5Winners { get; set; }
        public List<UserStatistics> Top5Loosers { get; set; }
        public int NumberOfEventsInCategory { get; set; }
        public int NumberOfRaffleParticipantsInCategory { get; set; }
        public int NumberOfEventTicketsHandedOut { get; set; }
        public int NumberOfEventTicketsNotHandedOut { get; set; }
        public int UniqueNumberOfRaffleParticipantsInCategory { get; set; }
        public int NumberOfTimesEventCreatorHasWonHisOwnRaffle { get; set; }
    }
}
