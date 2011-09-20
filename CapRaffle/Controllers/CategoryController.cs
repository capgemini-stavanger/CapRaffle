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
            if (ModelState.IsValid)
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

        public ViewResult Index()
        {
            var model = new CategoryListViewModel { Categories = repository.Categories };

            return View(model);
        }
    }
}
