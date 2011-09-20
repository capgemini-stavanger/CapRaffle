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
            User userExist = accountRepository.Users.FirstOrDefault(u => u.Email == model.Email);
            if (userExist == null)
            {
                ModelState.AddModelError("", "Email not found");
            }

            if (ModelState.IsValid)
            {
                if (accountRepository.ChangePassword(model.Email, model.Password))
                {
                    this.Success(string.Format("Password for {0} has been saved", model.Email));
                    return View(model);
                }
                else return View(model);
            }
            else
            {
                return View(model);
            }
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

        public ActionResult Index()
        {
           return Redirect("/");
        }

        public ActionResult SignOut()
        {
            accountRepository.SignOut();
            return Redirect("/Account/LogOn");
        }
    }
}
