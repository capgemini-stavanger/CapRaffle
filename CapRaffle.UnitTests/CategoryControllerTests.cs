using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using CapRaffle.Domain.Abstract;
using CapRaffle.Controllers;
using CapRaffle.Domain.Model;
using MvcContrib.TestHelper;
using System.Web.Mvc;
using CapRaffle.Models;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    public class CategoryControllerTests : Shared
    {

        [Test]
        public void Can_Create_New_Category()
        {
            ViewResult result = (ViewResult)categoryController.Create();

            result.AssertViewRendered().ForView("Edit");
            Assert.IsInstanceOf(typeof(Category), result.Model);
        }

        [Test]
        public void Can_Save_New_Category()
        {
            ViewResult target = (ViewResult)categoryController.Create();

            Category newCategory = target.Model as Category;
            newCategory.Name = "Soccer";

            var result = categoryController.Edit(newCategory);

            eventMock.Verify(m => m.SaveCategory(newCategory), Times.Once());
        }

        [Test]
        public void Can_Not_Save_Category_That_Already_Exists()
        {
            var category = new Category { Name = "Hockey" };

            var result = categoryController.Edit(category);

            eventMock.Verify(m => m.SaveCategory(It.IsAny<Category>()), Times.Never());
            result.AssertViewRendered().ForView(string.Empty);
        }

        [Test]
        public void Can_Not_Save_Category_With_Empty_Name()
        {
            var category = new Category { Name = null };

            var result = categoryController.Edit(category);

            eventMock.Verify(m => m.SaveCategory(It.IsAny<Category>()), Times.Never());
            result.AssertViewRendered().ForView(string.Empty);
        }

        [Test]
        public void Can_Display_All_Categories_From_Repository_Alphabetical()
        {
            var result = categoryController.Index();

            var categoryListViewModel = result.Model as CategoryListViewModel;

            eventMock.Verify(m => m.Categories, Times.Once());
            result.AssertViewRendered().ForView(string.Empty);
            Assert.AreEqual(eventMock.Object.Categories.Count(), categoryListViewModel.Categories.Count());
            Assert.AreEqual("A category", categoryListViewModel.Categories.First().Name); 
        }

        [Test]
        public void Can_Edit_Existing_Category()
        {
            var target = categoryController.Index();
            var categoryListViewModel = target.Model as CategoryListViewModel;


            var categoryToEdit = categoryListViewModel.Categories.First();
            ViewResult result = (ViewResult)categoryController.Edit(categoryToEdit.CategoryId);

            var categorySentToView = result.Model as Category;

            result.AssertViewRendered().ForView(string.Empty);
            Assert.AreEqual(categoryToEdit.Name, categorySentToView.Name);
            Assert.AreEqual(categoryToEdit.CategoryId, categorySentToView.CategoryId);
        }

        [Test]
        public void Can_Save_Edited_Category()
        {
            var target = categoryController.Index();
            var categoryListViewModel = target.Model as CategoryListViewModel;

            var categoryToEdit = categoryListViewModel.Categories.First();
            categoryToEdit.Name = "Hockey edited";

            var result = categoryController.Edit(categoryToEdit);

            result.AssertActionRedirect().ToAction("Index");
            eventMock.Verify(m => m.SaveCategory(categoryToEdit), Times.Once());
        }

        [Test]
        public void Can_Display_Category_Rules()
        {
            var result = categoryController.Rules(1);

            result.AssertPartialViewRendered().ForView("_Rules");
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }

        [Test]
        public void Can_Save_Category_Rules()
        {
            List<SaveRuleViewModel> rules = new List<SaveRuleViewModel>();
            SaveRuleViewModel srvm = new SaveRuleViewModel { RuleId = 1, Param = 10 };
            rules.Add(srvm);

            var result = categoryController.SaveRules(1, rules);

            Assert.IsInstanceOf(typeof(JsonResult), result);
        }
    }
}
