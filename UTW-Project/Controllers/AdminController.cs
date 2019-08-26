using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTW_Project.Classes;
using System.Web.Security;


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

        public ActionResult Activate(string username)
        {
            db.ActivateUser(username);
            return RedirectToAction("Users");
        }
    }
}