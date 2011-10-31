using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.Models;
using CapRaffle.ActionFilterAttributes;
using CapRaffle.Domain.Raffle;

namespace CapRaffle.Controllers
{
    [Authorize]
    [SetSelectedMenu]
    public class CategoryController : Controller
    {
        private IEventRepository repository;

        public CategoryController(IEventRepository repo)
        {
            repository = repo;
        }
  
        public ViewResult Index()
        {
            var model = new CategoryListViewModel { Categories = repository.Categories.OrderBy(c => c.Name).ToList() };
            
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
        [ValidateAntiForgeryToken]
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

        public PartialViewResult Rules(int categoryId)
        {
            var rulesforevent = repository.GetRulesForCategory(categoryId);
            var available = repository.AvailableRules.ToList();

            available.RemoveAll(x => rulesforevent.Exists(y => y.Rule.RuleId == x.RuleId));
            var model = new RulesViewModel
            {
                AvailableRules = available,
                RulesForEvent = rulesforevent,
                CategoryId = categoryId
            };

            return PartialView("_Rules", model);
        }

        public JsonResult SaveRules(int categoryId, List<SaveRuleViewModel> rules)
        {
            var ruleparameters = new List<RuleParameter>();
            if (rules != null)
            {
                rules = rules.Distinct(new RuleComparer()).ToList();
                rules.ForEach(x => ruleparameters.Add(new RuleParameter { Rule = new Rule { RuleId = x.RuleId }, Param = x.Param }));
            }
            repository.SaveRulesForCategory(categoryId, ruleparameters);
            return this.Json(true);
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
