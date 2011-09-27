using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Abstract
{
    public interface ICategoryRepository
    {
        IQueryable<Category> Categories { get; }

        void SaveCategory(Category category);

        int PreviousWinsInCategoryByUser(int categoryId, string email);
    }
}
