using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.Models;
using System.Reflection;
using CapRaffle.ActionFilterAttributes;

namespace CapRaffle.Controllers
{
    [SetSelectedMenu]
    public class AccountController : Controller
    {

        private IAccountRepository accountRepository;

        public AccountController(IAccountRepository accountRepos)
        {
            accountRepository = accountRepos;
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (accountRepository.ChangePassword(model.Email, model.Password))
                {
                    this.Success(string.Format("Password for {0} has been saved", model.Email));
                }
                else
                {
                    this.Error(string.Format("Password not changed"));
                }
            }
            return View("PersonalPage");
        }

        [Authorize]
        public ActionResult ChangePasswordPartial()
        {
            ChangePasswordViewModel user = new ChangePasswordViewModel
            {
                Email = HttpContext.User.Identity.Name
            };
            return PartialView("_ChangePassword", user);
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (accountRepository.Authenticate(model.Email, model.Password))
                {
                    return Redirect(String.IsNullOrEmpty(returnUrl) ? Url.Action("/"): returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect email or password");
                    TempData["ForgotPassword"] = "ForgotPassword";
                }
            }
            return View();
        }

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
               if (accountRepository.Create(user.Email, user.Password, user.Name))
                    {
                        this.Success(string.Format("Account for {0} has been registered", user.Email));
                        accountRepository.Authenticate(user.Email, user.Password);
                        return RedirectToAction("Index", "Event");
                    }
                }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Event");
        }

        public ActionResult SignOut()
        {
            accountRepository.SignOut();
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult EmailExists(string email)
        {
            bool isValid = true;
            User userExist = accountRepository.GetUserByEmail(email);
            if (userExist != null) isValid = false;
            return Json(isValid, JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public ActionResult ForgotPassword(LogOnViewModel model)
        {
            if (ModelState.IsValid)
            {
                User userExist = accountRepository.GetUserByEmail(model.Email);
                if (userExist != null)
                {
                    accountRepository.ForgotPassword(model.Email);
                    this.Success(string.Format("New password sent to {0}", model.Email));
                }
                else
                {
                    this.Error(string.Format("Email not found: {0}", model.Email));
                }
            }
            return RedirectToAction("LogOn", "Account");
        }

        [Authorize]
        public ActionResult PersonalPage()
        {
            User model = accountRepository.GetUserByEmail(HttpContext.User.Identity.Name);
            return View(model);
        }

        [Authorize]
        public ActionResult ChangeNamePartial()
        {
            User user = accountRepository.GetUserByEmail(HttpContext.User.Identity.Name);
            return PartialView("_ChangeName", new ChangeNameViewModel { NewName = user.Name } );
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangeName(ChangeNameViewModel model)
        {
            User userExist = accountRepository.GetUserByEmail(model.Email);
            if (userExist != null && model.NewName != null)
            {
                accountRepository.ChangeName(model.Email, model.NewName);
                this.Success(string.Format("Name changed to {0}", model.NewName));
            }
            else
            {
                this.Error(string.Format("Name not changed"));
            }
            return View("PersonalPage");
        }
    }
}
