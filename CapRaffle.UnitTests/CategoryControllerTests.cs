using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using CapRaffle.Domain.Abstract;
using CapRaffle.Controllers;
using CapRaffle.Domain.Model;

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
        }

        [Test]
        public void Can_Not_Create_Category_That_Already_Exists()
        {
            
        }
    }
}
