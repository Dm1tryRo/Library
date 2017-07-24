using OnlineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OnlineLibrary.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                Library lib = new Library();
                User user = null;
                user = lib.FindUser(login);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Name, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login not found");
                }
            }
            return View(login);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register reg)
        {
            if (ModelState.IsValid)
            {
                Library lib = new Library();
                User user = lib.FindUser(reg);
                if(user == null)
                {
                    lib.AddNewUser(reg);
                    if(lib.FindUser(reg) != null)
                    {
                        FormsAuthentication.SetAuthCookie(reg.Name, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User like this Email or name is already exists"); 
                }
            }
            return View(reg);
        }
    }
}