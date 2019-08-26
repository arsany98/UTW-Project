using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTW_Project.Models;
using UTW_Project.Classes;
using CaptchaMvc.HtmlHelpers;
using System.Data.Entity.Validation;
using System.Web.Security;


namespace UTW_Project.Controllers
{
    public class AccountController : Controller
    {

        private DBManager db = new DBManager();
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
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


                    if (user.LoginTrials>3 && user.LoginTrials<=5)
                    {
                        ViewBag.ShowCaptcha = 1;
                    }
                    else
                    {
                        ViewBag.ShowCaptcha = 0;
                    }

                    string EncryptedPassword = user.MD5Hash(password);
                    if ((EncryptedPassword!=user.Password) || (!this.IsCaptchaValid("") && (user.LoginTrials > 3 && user.LoginTrials <= 6)))
                    {
                        ViewBag.error = "Wrong username or password";
                        if (user.LoginTrials <= 5)
                        {
                            db.UpdateTrials(username);
                        }
                        else
                        {
                            user.Blocked = true;
                            db.UpdateTrials(username);
                        }
                  
                    }
                    else
                    {
                        db.ActivateUser(username);
                        FormsAuthentication.SetAuthCookie(username,false);
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

        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.QuestionID = db.GetQuestions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(User user)
        {
           
            ViewBag.QuestionID = db.GetQuestions();
            if (ModelState.IsValid)
            {
                bool usernameExists = db.UserExists(user.Username);

                bool emailExists = db.HasAccount(user.Email);
           
                if(!usernameExists && !emailExists)
                {
                    user.Admin = false;
                    user.Blocked = false;
                    user.EmailConfirmed = false;
                    user.LoginTrials = 2;
                    user.Wallet = 1000; /////////
                    user.Password = user.MD5Hash(user.Password);
                    user.Answer = user.MD5Hash(user.Answer);

                    try
                    {
                        db.Add(user);
                        string url = Url.Action("ConfirmEmail", "Account", new { user.Username }, Request.Url.Scheme);
                        EmailManager.SendConfirmationEmailEN(user, url);
                    }
                    catch(DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            ViewBag.error += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                ViewBag.error += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        
                        return View();                    
                    }
                    catch(Exception e)
                    {
                        ViewBag.error = e.Message;
                        return View();
                    }
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
            return View();
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ForgetPassword(string username)
        {
            if(db.UserExists(username))
            {
                User user = db.GetUser(username);
                if (user.EmailConfirmed == false)
                {
                    ViewBag.message = "You didn't confirm your Email yet.";
                }
                else
                {
                    string url = Url.Action("ResetPassword", "Account", new { user.Username }, Request.Url.Scheme);
                    EmailManager.SendResetPasswordEmailEN(user, url);
                    ViewBag.message = "An email has been sent to you to reset your password.";
                }
            }
            else
            {
                ViewBag.message = "username not found.";
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string username)
        {
            User user = db.GetUser(username);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ResetPassword(string username, string newPassword, string answer)
        {
            User user = db.GetUser(username);
            if (user.MD5Hash(answer) == user.Answer)
            {
                db.ResetPassword(user, newPassword);
                ViewBag.error = "Password reset successfully.";
            }
            else
            {
                ViewBag.error = "Wrong answer.";
            }
            return View(user);
        }

        [Authorize]
        public ActionResult AccountPage()
        {
            return View();
        }

    }
}