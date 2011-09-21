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
            }
           return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            ChangePasswordViewModel user = new ChangePasswordViewModel
            {
                Email = HttpContext.User.Identity.Name
            };
            return View(user);
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (accountRepository.Authenticate(model.Email, model.Password))
                {
                    return Redirect(returnUrl ?? Url.Action("/"));
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect email or password");
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
                        return Redirect("/");
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
           return Redirect("/");
        }

        public ActionResult SignOut()
        {
            accountRepository.SignOut();
            return Redirect("/Account/LogOn");
        }

        public ActionResult EmailExists(string email)
        {
            bool isValid = true;
            User userExist = accountRepository.GetUserByEmail(email);
            if (userExist != null) isValid = false;
            return Json(isValid, JsonRequestBehavior.AllowGet); 
        }
    }
}
