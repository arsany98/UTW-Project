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
                        ViewBag.error = "You didn't confirm your Email yet.";
                        return View();
                    }

                    if (user.Blocked)
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
            ViewBag.QuestionID = db.GetQuestions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {

            if (ModelState.IsValid)
            {
                bool usernameExists = db.UserExists(user.Username);

                bool emailExists = db.HasAccount(user.Email);
           
                if(!usernameExists && !emailExists)
                {
                    user.Admin = false;
                    user.Blocked = false;
                    user.EmailConfirmed = false;
                    user.LoginTrials = 0;
                    user.Wallet = 1000;
                    user.Password = user.MD5Hash(user.Password);

                    try
                    {
                        string url = Url.Action("ConfirmEmail", "Account", new { user.Username }, Request.Url.Scheme);
                        EmailManager.SendConfirmationEmailEN(user, url);
                    }
                    catch(Exception e)
                    {
                        ViewBag.error = e.Message;
                        return View();                    
                    }
                    db.Add(user);
                    return RedirectToAction("AccountPage");
                }
                else if (usernameExists)
                {
                    ViewBag.error = "Username Taken";
                    return View();
                }
                else if (emailExists)
                {
                    ViewBag.error = "Email has been used to create another account";
                    return View();
                }
            }
            ViewBag.QuestionID = db.GetQuestions();
            return View();
        }

        public ActionResult ConfirmEmail(string username)
        {
            if(db.EmailConfirm(username))
            { 
                ViewBag.Message = "Confirmed Successfully";
            }
            else
            {
                ViewBag.Message = "Error Confirming";
            }
            return View();
        }

        public ActionResult ForgetPassword(string username)
        {
            User user = db.GetUser(username);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult ForgetPassword(string username, string newPassword, string answer)
        {
            db.ResetPassword(username, newPassword, answer);
            return View();
        }


        public ActionResult ResetPassword(string username)
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string username, string newPassword, string answer)
        {
            
            return View();
        }


        public ActionResult AccountPage()
        {
            return View();
        }

    }
}