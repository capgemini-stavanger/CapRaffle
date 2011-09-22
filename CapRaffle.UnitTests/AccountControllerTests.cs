using System.Web.Mvc;
using CapRaffle.Controllers;
using CapRaffle.Domain.Abstract;
using CapRaffle.Models;
using Moq;
using NUnit.Framework;
using System.Linq;
using CapRaffle.Domain.Model;

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
            mock.Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mock.Setup(m => m.ChangePassword("test@capgemini.com", It.IsAny<string>())).Returns(true);
            mock.Setup(m => m.GetUserByEmail("test@capgemini.com")).Returns(new User() { Email = "testc@capgemini.com" });
            mock.Setup(m => m.Users).Returns(new User[] 
            {
                new User { Email="test@capgemini.com", Name="Test" },
                new User { Email="test2@capgemini.com", Name="Test2" }
            }.AsQueryable());

            var mockContext = new Mock<ControllerContext>();
            mockContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("test@capgemini.com");
            mockContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            accountController = new AccountController(mock.Object);
            accountController.ControllerContext = mockContext.Object;
        }

        [Test]
        public void Can_Register_With_Capgemini_Email()
        {
            //Arrange
            User model = new User
            {
                Email = "test3@capgemini.com",
                Password = "WeAreTheOnes",
                PasswordAgain = "WeAreTheOnes",
                Name = "name"
            };

            // Act
            ActionResult res = accountController.Register(model);

            // Assert
            mock.Verify(m => m.Create(model.Email, model.Password, model.Name));
            Assert.IsInstanceOf(typeof(RedirectResult), res);
        }

        [Test]
        public void Can_Not_Register_Without_Valid_Email()
        {
            //Arrange
            User model = new User
            {
                Email = "test",
                Password = "12"
            };

            // Act
            accountController.ModelState.AddModelError("error", "Email must end with @capgemini.com");
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
            ActionResult res = accountController.LogOn(model,"/Index");

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
            ActionResult res = accountController.LogOn(model,"/Index");

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), res);
            Assert.IsFalse(((ViewResult)res).ViewData.ModelState.IsValid);
        }

        [Test]
        public void Can_Change_Password()
        {
            // Arrange
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Email = "test@capgemini.com",
                Password = "newPass123",
                PasswordAgain = "newPass123"
            };

            

            // Act
            ActionResult res = accountController.ChangePassword(model);

            // Assert
            mock.Verify(m => m.ChangePassword(model.Email, model.Password));
            Assert.IsInstanceOf(typeof(ViewResult), res);
        }
        
        [Test]
        public void Can_Start_Change_Password_With_Correct_Email()
        {
            // Arrange


            // Act
            ActionResult res = accountController.ChangePassword();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), res);
        }

        [Test]
        public void Can_Not_Change_Password()
        {
            // Arrange
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Email = "test2@capgemini.com",
                Password = "newPass123",
                PasswordAgain = "newPass123"
            };

            // Act
            ActionResult res = accountController.ChangePassword(model);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), res);
        }

        [Test]
        public void Can_Sign_Off()
        {
            //Act
            ActionResult res = accountController.SignOut();

            //Assert
            Assert.IsInstanceOf(typeof(RedirectResult), res);
            mock.Verify(m => m.SignOut());
        }

        [Test]
        public void LogOn_Returns_View()
        {
            //Act
            ActionResult res = accountController.LogOn();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), res);
        }

        [Test]
        public void Register_Returns_View()
        {
            //Act
            ActionResult res = accountController.Register();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), res);
        }

        [Test]
        public void Index_Redirects_To_Main_Index()
        {
            //Act
            ActionResult res = accountController.Index();

            //Assert
            Assert.IsInstanceOf(typeof(RedirectResult), res);
            Assert.AreEqual("/", ((RedirectResult)res).Url);
        }

        [Test]
        public void Get_Valid_User_By_Email()
        {
            //Arrange
            string email = "test@capgemini.com";

            //Act
            ActionResult res = accountController.EmailExists(email);
            //Assert
            Assert.IsInstanceOf(typeof(JsonResult), res);
        }

        [Test]
        public void Forgot_Password_Working()
        {
            // Arrange
            LogOnViewModel model = new LogOnViewModel
            {
                Email = "test@capgemini.com",
                Password = "wrongpassword"
            };
            
            // Act
            ActionResult res = accountController.ForgotPassword(model);

            // Assert
            mock.Verify(m => m.ForgotPassword(model.Email));
            Assert.IsInstanceOf(typeof(RedirectResult), res);
        }
    }
}
