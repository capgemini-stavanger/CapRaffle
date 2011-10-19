using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;
using CapRaffle.Domain.Implementation;
using CapRaffle.Domain.Statistics;

namespace CapRaffle.Domain.Abstract
{
    public interface IStatisticRepository
    {

        CategoryStatistics CategoryStatistics(int categoryId);
        UserStatistics UserStatistics(string email);
    }
}
