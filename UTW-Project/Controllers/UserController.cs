using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace UTW_Project.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        // GET: User
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

    }
}