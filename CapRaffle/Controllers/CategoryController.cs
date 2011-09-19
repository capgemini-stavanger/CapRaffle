using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.Models;

namespace CapRaffle.Controllers
{
    public class CategoryController : Controller
    {
        private ICategoryRepository repository;

        public CategoryController(ICategoryRepository repo)
        {
            repository = repo;
        }

        [HttpPost]
        public ViewResult Create(Category newCategory)
        {
            Category category = repository.Categories
                .FirstOrDefault(c => c.Name == newCategory.Name);

            if (category == null)
            {
                repository.SaveCategory(newCategory);
                return View("Index");
            }
            
            return View();
        }

        public ViewResult Index()
        {
            //var categories = repository.Categories;
            //CategoryListViewModel categoryList = new CategoryListViewModel();
            //CategoryViewModel categoryViewModel = null;
            //foreach (var category in categories)
            //{
            //    categoryViewModel = new CategoryViewModel
            //    {
            //        CategoryId = category.CategoryId,
            //        Name = category.Name
            //    };
            //    categoryList.Categories.ToList() = categoryViewModel;
            //}

            var categories = repository.Categories.ToList();
            var categoryViewModel = new List<CategoryViewModel>();
            categories.ForEach(x => categoryViewModel.Add(new CategoryViewModel { Name = x.Name }));
            var model = new CategoryListViewModel { Categories = categoryViewModel.AsQueryable() };

            return View(model);
        }
    }
}
