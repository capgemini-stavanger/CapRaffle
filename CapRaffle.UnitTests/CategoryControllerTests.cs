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
    public class CategoryControllerTests
    {
        Mock<IEventRepository> mock;
        CategoryController controller;

        [SetUp]
        public void Setup()
        {
            mock = new Mock<IEventRepository>();
            mock.Setup(m => m.Categories).Returns(new Category[] 
            {
                new Category { CategoryId = 1, Name = "Hockey" },
                new Category { CategoryId = 2, Name = "A category" },
                new Category { CategoryId = 3, Name = "Other" }
            }.AsQueryable());
            
            controller = new CategoryController(mock.Object);
        }

        [Test]
        public void Can_Create_New_Category()
        {
            ViewResult result = (ViewResult)controller.Create();

            result.AssertViewRendered().ForView("Edit");
            Assert.IsInstanceOf(typeof(Category), result.Model);
        }

        [Test]
        public void Can_Save_New_Category()
        {
            ViewResult target = (ViewResult)controller.Create();

            Category newCategory = target.Model as Category;
            newCategory.Name = "Soccer";

            var result = controller.Edit(newCategory);

            mock.Verify(m => m.SaveCategory(newCategory), Times.Once());
        }

        [Test]
        public void Can_Not_Save_Category_That_Already_Exists()
        {
            var category = new Category { Name = "Hockey" };

            var result = controller.Edit(category);

            mock.Verify(m => m.SaveCategory(It.IsAny<Category>()), Times.Never());
            result.AssertViewRendered().ForView(string.Empty);
        }

        [Test]
        public void Can_Not_Save_Category_With_Empty_Name()
        {
            var category = new Category { Name = null };

            var result = controller.Edit(category);

            mock.Verify(m => m.SaveCategory(It.IsAny<Category>()), Times.Never());
            result.AssertViewRendered().ForView(string.Empty);
        }

        [Test]
        public void Can_Display_All_Categories_From_Repository_Alphabetical()
        {
            var result = controller.Index();

            var categoryListViewModel = result.Model as CategoryListViewModel;

            mock.Verify(m => m.Categories, Times.Once());
            result.AssertViewRendered().ForView(string.Empty);
            Assert.AreEqual(mock.Object.Categories.Count(), categoryListViewModel.Categories.Count());
            Assert.AreEqual("A category", categoryListViewModel.Categories.First().Name); 
        }

        [Test]
        public void Can_Edit_Existing_Category()
        {
            var target = controller.Index();
            var categoryListViewModel = target.Model as CategoryListViewModel;


            var categoryToEdit = categoryListViewModel.Categories.First();            
            ViewResult result = (ViewResult)controller.Edit(categoryToEdit.CategoryId);

            var categorySentToView = result.Model as Category;

            result.AssertViewRendered().ForView(string.Empty);
            Assert.AreEqual(categoryToEdit.Name, categorySentToView.Name);
            Assert.AreEqual(categoryToEdit.CategoryId, categorySentToView.CategoryId);
        }

        [Test]
        public void Can_Save_Edited_Category()
        {
            var target = controller.Index();
            var categoryListViewModel = target.Model as CategoryListViewModel;

            var categoryToEdit = categoryListViewModel.Categories.First();
            categoryToEdit.Name = "Hockey edited";

            var result = controller.Edit(categoryToEdit);

            result.AssertActionRedirect().ToAction("Index");
            mock.Verify(m => m.SaveCategory(categoryToEdit), Times.Once());
        }
    }
}
