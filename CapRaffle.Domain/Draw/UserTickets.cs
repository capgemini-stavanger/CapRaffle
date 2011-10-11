using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapRaffle.Domain.Draw
{
    public class UserTickets
    {
        public string Email { get; set; }
        public int NumberOfTickets { get; set; }

        public UserTickets(string e, int number) 
        {
            Email = e;
            NumberOfTickets = number;
        }
    }
}
