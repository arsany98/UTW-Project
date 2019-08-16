using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTW_Project.Models;
using UTW_Project.Classes;
namespace UTW_Project.Controllers
{
    public class AccountController : Controller
    {

        private DBManager db = new DBManager();
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {            

            if (username!="" && password!="")
            {

                User user = db.GetUser(username);
                if (user==null)
                {
                    ViewBag.error = "Wrong username or password";
                   
                }
                else
                {

                    if (user.EmailConfirmed == false)
                    {
                        ViewBag.error = "Wrong username or password";
                        return View();
                    }

                    if (user.LoginTrials > 3)
                    {
                        ViewBag.error = "You are blocked";
                        return View();
                    }


                    string EncryptedPassword = user.MD5Hash(password);
                    if (EncryptedPassword!=user.Password)
                    {
                        ViewBag.error = "Wrong username or password";
                        if (user.LoginTrials < 3)
                        {
                            db.UpdateTrials(username);
                        }
                        else if (user.LoginTrials == 3)
                        {
                            user.Blocked = true;
                            db.UpdateTrials(username);
                        }
                  
                    }
                    else
                    {
                        return RedirectToAction("AccountPage");
                    }

                }


            }
            else
            {
                ViewBag.error = "You must enter both username and password";
            }

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {

            bool c = db.UserExists(user.Username);

            bool e = db.HasAccount(user.Email);

            

            if (c == false && !e && ModelState.IsValid)
            {

                user.Q_ID = 1;
                user.Admin = false;
                user.Blocked = false;
                user.EmailConfirmed = false;
                user.LoginTrials = 0;
                user.Wallet = 1000;
                user.Password = user.MD5Hash(user.Password);
                db.Add(user);

            }
            else if (ModelState.IsValid && c==true)
            {
                ViewBag.error = "Username Taken";
                return View();
            }
            else if (ModelState.IsValid && e)
            {
                ViewBag.error = "Email has been used to create another account";
                return View();
            }

            if (ModelState.IsValid)
                return RedirectToAction("AccountPage");

            return View();
        }

        public ActionResult ConfirmEmail()
        {
            return View();
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword(string newPassword, string answer)
        {
            return View();
        }


        public ActionResult AccountPage()
        {
            return View();
        }


    }
}