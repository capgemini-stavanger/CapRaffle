using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapRaffle.Domain.Model;

namespace CapRaffle.Models
{
    public class DrawWinnerViewModel
    {
        public int NumberOfSpotsLeft { get; set; }

        public List<Winner> Winners { get; set; }
    }
}