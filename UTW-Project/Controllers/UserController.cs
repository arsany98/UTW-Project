using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataAccessLayer;
using BussinessLayer;

namespace UTW_Project.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        // GET: User
        private DBManager db = new DBManager();
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Order(string username, string type, string stockName, int quantity)
        {
            if (!db.AddOrder(username, type, stockName, quantity))
            {
                ViewBag.error = "You don't have enough money!";
            }
            return View();
        }

        public ActionResult Monitor()
        {
            string id = this.HttpContext.User.Identity.Name;
            int userID = Convert.ToInt32(id);
            return View(db.getUserTransactions(userID)); //transactions for user
        }

        [HttpPost]
        public ActionResult Monitor(DateTime startDate, DateTime endDate)
        {
            string id = this.HttpContext.User.Identity.Name;
            int userID = Convert.ToInt32(id);

            return View(db.getUserTransactions(userID,startDate,endDate)); //transactions for user
        }

        [HttpPost]
        public ActionResult Monitor(DateTime startDate, DateTime endDate, string stock)
        {
            string id = this.HttpContext.User.Identity.Name;
            int userID = Convert.ToInt32(id);

            return View(db.getUserTransactions(userID, stock, startDate, endDate)); //transactions for user
        }

        [HttpPost]
        public ActionResult Monitor(string stock)
        {
            string id = this.HttpContext.User.Identity.Name;
            int userID = Convert.ToInt32(id);

            return View(db.getUserTransactions(userID, stock)); //transactions for user
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}