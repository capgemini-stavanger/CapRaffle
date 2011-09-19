using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;

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
            return View("Index", repository.Categories);
        }
    }
}
