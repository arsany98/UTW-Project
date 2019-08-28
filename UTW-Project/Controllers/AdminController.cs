using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTW_Project.Classes;
using System.Web.Security;
using UTW_Project.Models;


namespace UTW_Project.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private DBManager db = new DBManager();

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Order()
        {
            return View();
        }
        public ActionResult Monitor()
        {
            return View();
        }

        public ActionResult Users()
        {
            var users = db.GetUsersList();
            return View(users);
        }
        [HttpPost]
        public ActionResult Users(string selectedMethod, string filterValue)
        {

            List<User> users= new List<User>();

            if (filterValue=="" && selectedMethod!= "Default sorting")
            {
                ViewBag.error = "You have to enter a value to filter with";
                return View(users);
            }

            if (selectedMethod == "Sort by Status(active or blocked)")
            {
                if (filterValue=="blocked")
                {
                    users = db.SelectByStatues(true);

                }
                else if (filterValue=="active")
                {
                    users = db.SelectByStatues(false);
                }

            }
            else if (selectedMethod== "Sort by Email")
            {
                users.Add(db.SelectByEmail(filterValue));
            }
            else if (selectedMethod== "Sort by Username")
            {
                users.Add(db.SelectByUsername(filterValue));
            }
            else
            {
                users = db.GetUsersList();
            }


            return View(users);
        }

        public ActionResult Activate(string username)
        {
            db.ActivateUser(username);
            return RedirectToAction("Users");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}