using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;

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
            if (category.CategoryId == 0)
            {
                context.AddToCategories(category);
            }
            else
            {
                context.UpdateDetachedEntity<Category>(category, x => x.CategoryId);
            }
            context.SaveChanges();
        }
    }
}
