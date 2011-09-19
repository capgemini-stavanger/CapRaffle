using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        CapRaffleContext context = new CapRaffleContext();

        public IQueryable<Category> Categories
        {
            get { return context.Categories; }
        }

        public void SaveCategory(Category category)
        {
            context.AddToCategories(category);
            context.SaveChanges();
        }
    }
}
