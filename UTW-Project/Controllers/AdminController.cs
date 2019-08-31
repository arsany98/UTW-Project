using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BussinessLayer;
using System.Web.Security;
using DataAccessLayer;


namespace UTW_Project.Controllers
{
    [Authorize(Users = "admin")]
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
            return View(db.getUserTransactions());
        }
        
        [HttpPost]
        public ActionResult Monitor(int userID, DateTime startDate, DateTime endDate, string stock)
        {
            if (userID != null && startDate != null && endDate != null && stock != null)
            {
                return View(db.getUserTransactions(userID, stock, startDate, endDate));
            } //transactions for user
            else if (userID == null && startDate != null && endDate != null && stock != null)
            {
                return View(db.getUserTransactions(stock, startDate, endDate));
            }
            ///
            else if (userID != null && startDate == null && endDate != null && stock != null)
            {
                return View(db.getUserTransactions(userID, stock));
            }
            else if (userID != null && startDate != null && endDate == null && stock != null)
            {
                return View(db.getUserTransactions(userID, stock));
            }
            else if (userID != null && startDate != null && endDate != null && stock == null)
            {
                return View(db.getUserTransactions(userID, startDate,endDate));
            }
            else if (userID == null && startDate == null && endDate != null && stock != null)
            {
                return View(db.getUserTransactions(stock));
            }
            else if (userID != null && startDate == null && endDate == null && stock != null)
            {
                return View(db.getUserTransactions(userID, stock));
            }
            else if (userID != null && startDate != null && endDate == null && stock == null)
            {
                return View(db.getUserTransactions(userID));
            }
            else if (userID != null && startDate != null && endDate != null && stock == null)
            {
                return View(db.getUserTransactions(userID, startDate, endDate));
            }
            else if (userID == null && startDate != null && endDate != null && stock != null)
            {
                return View(db.getUserTransactions(stock,startDate, endDate));
            }
            else
            {
                return View(db.getUserTransactions());
            }
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
            return RedirectToAction("Login", "Account");
        }

      
    }
}