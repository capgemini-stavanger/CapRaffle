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
    [Authorize]
    public class CategoryController : Controller
    {
        private ICategoryRepository repository;

        public CategoryController(ICategoryRepository repo)
        {
            repository = repo;
        }
  
        public ViewResult Index()
        {
            var model = new CategoryListViewModel { Categories = repository.Categories.OrderBy(c => c.Name) };
            
            return View(model);
        }

        public ViewResult Create()
        {
            ViewBag.action = "Create";
            return View("Edit", new Category());
        }
        
        public ViewResult Edit(int categoryId)
        {
            Category category = repository.Categories.FirstOrDefault(
                f => f.CategoryId == categoryId);
            ViewBag.action = "Edit";
            return View(category);
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelStateAndCategoryNameIsValid(category))
            {
                if (CategoryAlreadyExists(category))
                {
                    this.Error("A category with that name already exists.");
                    //ModelState.AddModelError("", "A category with that name already exists.");
                    return View(category);
                }
                else
                {
                    repository.SaveCategory(category);
                    this.Success("The category has been saved");
                    return RedirectToAction("Index");
                }
            }            
            return View(category);
        }

        private bool ModelStateAndCategoryNameIsValid(Category category)
        {
            return ModelState.IsValid && !string.IsNullOrEmpty(category.Name);
        }

        private bool CategoryAlreadyExists(Category category)
        {
            Category existingCategory = repository.Categories
                    .FirstOrDefault(c => c.Name == category.Name);

            return existingCategory != null && category.CategoryId == 0;
        }
    }
}
