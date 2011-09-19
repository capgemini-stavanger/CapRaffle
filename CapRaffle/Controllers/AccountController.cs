using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
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
                    TempData["message"] = string.Format("{0} has been saved", email); //Display in view.
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
        public ActionResult LogOn(LogOnViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (accountRepository.Authenticate(model.Email, model.Password))
                {
                    return Redirect("/Index");
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
                ModelState.AddModelError("", "Incorrect email or password"); //Possibly not needed, the view should add the modelerror when the RegularExpression fails?
           
            if (ModelState.IsValid)
            {
                if (accountRepository.Create(model.Email, model.Password))
                {
                    return Redirect("/Registered");
                }
                else
                {
                    return View();

                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult Register()
        {
            return View();
        }
    }
}
