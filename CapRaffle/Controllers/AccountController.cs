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

        [HttpPost]
        public ActionResult Delete(string email)
        {
            if (ModelState.IsValid)
            {
                if(accountRepository.Delete(email))
                {
                    TempData["message"] = string.Format("{0} has been deleted", email); //Display in view.
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string email, string newPassword)
        {
            if (ModelState.IsValid)
            {
                if (accountRepository.ChangePassword(email, newPassword))
                {
                    TempData["message"] = string.Format("{0} has been saved", email); //Display in view.
                    return RedirectToAction("Index");
                }
                else return View();
            }
            else
            {
                return View();
            }
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (accountRepository.Authenticate(model.Email, model.Password))
                {
                    return Redirect(returnUrl ?? Url.Action("Index"));
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect email or password");
                    return View();

                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult LogOn()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!model.Email.Contains("@capgemini.com"))
            {
                ModelState.AddModelError("", "Email must end with @capgemini.com"); //Possibly not needed, the view should add the modelerror when the RegularExpression fails?
            }

            if (model.Password != model.PasswordAgain)
            {
                ModelState.AddModelError("", "Password did not match"); //Possibly not needed, the view should add the modelerror when the RegularExpression fails?
            }

            User userExist = accountRepository.Users.FirstOrDefault(u => u.Email == model.Email);
            if (userExist != null)
            {
                ModelState.AddModelError("", "Email is already registered");
            }
            
            if (ModelState.IsValid)
            {
               if (accountRepository.Create(model.Email, model.Password, model.Name))
                    {
                        return Redirect("/Account/LogOn");
                    }
                }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
    }
}
