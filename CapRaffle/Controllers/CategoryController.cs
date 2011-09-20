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

        public ViewResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public ActionResult Create(Category newCategory)
        {            
            if (ModelStateAndCategoryNameIsValid(newCategory))
            {
                Category category = repository.Categories
                    .FirstOrDefault(c => c.Name == newCategory.Name);

                if (category == null)
                {
                    repository.SaveCategory(newCategory);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "A category with that name already exists.");
                    return View();
                }
            }            
            return View();
        }

        private bool ModelStateAndCategoryNameIsValid(Category category)
        {
            return ModelState.IsValid && !string.IsNullOrEmpty(newCategory.Name);
        }

        public ViewResult Index()
        {
            var model = new CategoryListViewModel { Categories = repository.Categories };
            model.Categories.OrderBy(p => p.Name);

            return View(model);
        }
    }
}
