using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BusinessLayer;
namespace UTW_Project.Controllers
{ 
    public class BaseController : Controller
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string culture;
            if (this.Session == null || this.Session["CurrentCulture"] == null)
            {
                culture = "en";
                this.Session["CurrentCulture"] = culture;
            }
            else
            {
                culture = Session["CurrentCulture"] as string;
            }

            DBManager db = new DBManager();
            this.Session["User"] = db.GetUser(User.Identity.Name);
            CultureManager.CurrentCulture = culture;
            return base.BeginExecuteCore(callback, state);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            DBManager db = new DBManager();
            this.Session["User"] = db.GetUser(User.Identity.Name);
            base.OnActionExecuted(filterContext);
        }

    }
}