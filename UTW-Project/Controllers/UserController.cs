﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UTW_Project.Classes;

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

      

        public ActionResult Logout()
        {
            Session["User"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}