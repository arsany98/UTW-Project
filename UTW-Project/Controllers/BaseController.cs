using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using UTW_Project.Classes;

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
            CultureManager.CurrentCulture = culture;
            return base.BeginExecuteCore(callback, state);
        }
       
    }
}