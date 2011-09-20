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
        Mock<ICategoryRepository> mock;
        CategoryController controller;

        [SetUp]
        public void Setup()
        {
            mock = new Mock<ICategoryRepository>();
            mock.Setup(m => m.Categories).Returns(new Category[] 
            {
                new Category { Name = "Hockey" },
                new Category { Name = "Other" }
            }.AsQueryable());
            
            controller = new CategoryController(mock.Object);
        }

        [Test]
        public void Can_Create_New_Category()
        {           
            var newCategory = new Category { Name = "Soccer" };

            var result = controller.Create(newCategory);
            
            mock.Verify(m => m.SaveCategory(newCategory), Times.Once());
            result.AssertActionRedirect().ToAction("Index");
        }

        [Test]
        public void Can_Not_Create_Category_That_Already_Exists()
        {
            var newCategory = new Category { Name = "Hockey" };

            var result = controller.Create(newCategory);

            mock.Verify(m => m.SaveCategory(It.IsAny<Category>()), Times.Never());
            result.AssertViewRendered().ForView(string.Empty);
        }

        [Test]
        public void Can_Display_All_Categories_From_Repository()
        {
            var result = controller.Index();

            var categoryListViewModel = result.Model as CategoryListViewModel;

            mock.Verify(m => m.Categories, Times.Once());
            result.AssertViewRendered().ForView(string.Empty);
            Assert.AreEqual(2, categoryListViewModel.Categories.Count());
        }
    }
}
