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
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}