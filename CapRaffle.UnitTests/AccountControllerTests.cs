using System.Web.Mvc;
using CapRaffle.Controllers;
using CapRaffle.Domain.Abstract;
using CapRaffle.Models;
using Moq;
using NUnit.Framework;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    class AccountControllerTests
    {

        Mock<IAccountRepository> mock;
        AccountController accountController;

        [SetUp]
        public void Setup()
        {
            mock = new Mock<IAccountRepository>();
            mock.Setup(m => m.Authenticate("test@capgemini.com", "pass1234")).Returns(true);
            mock.Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            accountController = new AccountController(mock.Object);
        }

        [Test]
        public void Can_Register_With_Capgemini_Email()
        {
            //Arrange
            RegisterViewModel model = new RegisterViewModel
            {
                Email = "test@capgemini.com",
                Password = "WeAreTheOnes"
            };

            // Act
            ActionResult res = accountController.Register(model);

            // Assert
            Assert.IsInstanceOf(typeof(RedirectResult), res);
            Assert.AreEqual("/Registered", ((RedirectResult)res).Url);
        }

        [Test]
        public void Can_Not_Register_Without_Valid_Email()
        {
            //Arrange
            RegisterViewModel model = new RegisterViewModel
            {
                Email = "test",
                Password = "12"
            };

            // Act
            ActionResult res = accountController.Register(model);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), res);
            Assert.IsFalse(((ViewResult)res).ViewData.ModelState.IsValid);
        }

        [Test]
        public void Can_Log_In()
        {
            // Arrange
            LogOnViewModel model = new LogOnViewModel
            {
                Email = "test@capgemini.com",
                Password = "pass1234"
            };

            // Act
            ActionResult res = accountController.LogOn(model);

            // Assert
            Assert.IsInstanceOf(typeof(RedirectResult), res);
            Assert.AreEqual("/Index", ((RedirectResult)res).Url);
        }

        [Test]
        public void Can_Not_Log_In()
        {
            // Arrange
            LogOnViewModel model = new LogOnViewModel
            {
                Email = "test@capgemini.com",
                Password = "wrongpassword"
            };

            // Act
            ActionResult res = accountController.LogOn(model);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), res);
            Assert.IsFalse(((ViewResult)res).ViewData.ModelState.IsValid);
        }
    }
}
