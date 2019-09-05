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
    public class HomeController : BaseController
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
            if (username != "" && password != "")
            {

                User user = db.GetUser(username);
                if (user == null)
                {
                    ViewBag.error = Resources.Resources.WrongUsernameOrPassword;
                }
                else
                {

                    if (user.EmailConfirmed == false)
                    {
                        ViewBag.error = Resources.Resources.EmailNotConfirmed;
                        return View();
                    }

                    if (user.Blocked)
                    {
                        ViewBag.error = Resources.Resources.YouAreBlocked;
                        return View();
                    }


                    if (user.LoginTrials > 1 && user.LoginTrials <= 3)
                    {
                        ViewBag.ShowCaptcha = 1;
                    }
                    else
                    {
                        ViewBag.ShowCaptcha = 0;
                    }
                    string EncryptedPassword = user.MD5Hash(password);
                    if (user.Admin)
                    {
                        if (EncryptedPassword != user.Password || (!this.IsCaptchaValid("") && (user.LoginTrials > 1 && user.LoginTrials <= 4)))
                        {
                            ViewBag.error = Resources.Resources.WrongUsernameOrPassword;
                            if (user.LoginTrials <= 3)
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
                            FormsAuthentication.SetAuthCookie(user.Username, false);
                            Session["User"] = user;
                            return RedirectToAction("Dashboard", "Home");
                        }
                    }
                    else
                    {
                        
                        if ((EncryptedPassword != user.Password) || (!this.IsCaptchaValid("") && (user.LoginTrials > 1 && user.LoginTrials <= 4)))
                        {
                            ViewBag.error = Resources.Resources.WrongUsernameOrPassword;
                            if (user.LoginTrials <= 3)
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
                            FormsAuthentication.SetAuthCookie(username, false);
                            Session["User"] = user;
                            return RedirectToAction("Dashboard");
                        }
                    }
                }

            }
            else
            {
                ViewBag.error = Resources.Resources.EmptyLoginFields;
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

                if (!usernameExists && !emailExists)
                {
                    user.Admin = false;
                    user.Blocked = false;
                    user.EmailConfirmed = false;
                    user.LoginTrials = 0;
                    user.Wallet = 1000; /////////
                    user.Password = user.MD5Hash(user.Password);
                    user.Answer = user.MD5Hash(user.Answer);

                    try
                    {
                        db.Add(user);
                        Guid guid = Guid.NewGuid();
                        string url = Url.Action("ConfirmEmail", "Home", new { guid }, Request.Url.Scheme);
                        db.AddURL(url, guid, user);
                        if(System.Threading.Thread.CurrentThread.CurrentCulture.Name == "en")
                            EmailManager.SendConfirmationEmailEN(user);
                        else
                            EmailManager.SendConfirmationEmailAR(user);
                    }
                    catch (Exception e)
                    {
                        ViewBag.error = e.Message;
                        return View();
                    }
                    return RedirectToAction("Dashboard");
                }
                else if (usernameExists)
                {
                    ViewBag.error = Resources.Resources.UsernameTaken;
                    return View();
                }
                else if (emailExists)
                {
                    ViewBag.error = Resources.Resources.EmailTaken;
                    return View();
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(Guid guid)
        {
            User user = db.GetUrlUser(guid);
            if (user.URL.Expired)
            {
                ViewBag.message = Resources.Resources.ExpiredUrl;
            }
            else if (db.EmailConfirm(user))
            {
                ViewBag.Message = Resources.Resources.ConfirmedSuccessfully;
            }
            else
            {
                ViewBag.Message = Resources.Resources.ErrorConfirming;
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
            if (db.UserExists(username))
            {
                User user = db.GetUser(username);
                if (user.EmailConfirmed == false)
                {
                    ViewBag.message = Resources.Resources.EmailNotConfirmed;
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    string url = Url.Action("ResetPassword", "Home", new { guid }, Request.Url.Scheme);
                    db.AddURL(url, guid, user);
                    if (System.Threading.Thread.CurrentThread.CurrentCulture.Name == "en")
                        EmailManager.SendResetPasswordEmailEN(user);
                    else
                        EmailManager.SendResetPasswordEmailAR(user);
                    ViewBag.message = Resources.Resources.ResetPasswordEmailSent;
                }
            }
            else
            {
                ViewBag.message = Resources.Resources.UsernameNotFound;
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(Guid guid)
        {
            User user = db.GetUrlUser(guid);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ResetPassword(Guid guid, string newPassword, string answer)
        {
            User user = db.GetUrlUser(guid);
            if (user.MD5Hash(answer) == user.Answer)
            {
                db.ResetPassword(user, newPassword);
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.error = Resources.Resources.WrongAnswer;
            }
            return View(user);
        }
        [Authorize]
        public ActionResult Monitor()
        {
            // string id = this.HttpContext.User.Identity.Name;
            // int userID = Convert.ToInt32(id);
            var user = Session["User"] as User;
            if (user.Admin == true)
            {
                return View(db.getUserTransactions());
            }
            else
                return View(db.getUserTransactions(user.ID)); //transactions for user
        }

        [Authorize]
        [HttpPost]
        public ActionResult Monitor(int userID, DateTime? startDate, DateTime? endDate, int stockID)
        {
            var user = Session["User"] as User;
            string stock;
            if (stockID == 0)
            {
                stock = "all";
            }
            else
            {
                stock = db.GetStock(stockID).CompanyEN;
            }
            if (user.Admin == true)
            {
                //no nulls
                if (userID != 0 && startDate != null && endDate != null && stock != "all")
                {
                    return View(db.getUserTransactions(userID, stock, startDate, endDate));
                }
                //one null
                else if (userID == 0 && startDate != null && endDate != null && stock != "all")
                {
                    return View(db.getUserTransactions(stock, startDate, endDate));
                }
                else if (userID != 0 && startDate == null && endDate != null && stock != "all")
                {
                    var startD = new DateTime(1850, 1, 1);
                    return View(db.getUserTransactions(userID, stock, startD, endDate));
                }
                else if (userID != 0 && startDate != null && endDate == null && stock != "all")
                {
                    return View(db.getUserTransactions(userID, stock, startDate, DateTime.Now));
                }
                else if (userID != 0 && startDate != null && endDate != null && stock == "all")
                {
                    return View(db.getUserTransactions(userID, startDate, endDate));
                }
                //two nulls
                else if (userID == 0 && startDate == null && endDate != null && stock != "all")
                {
                    var startD = new DateTime(1850, 1, 1);
                    return View(db.getUserTransactions(stock, startD, endDate));
                }
                else if (userID != 0 && startDate == null && endDate == null && stock != "all")
                {
                    return View(db.getUserTransactions(userID, stock));
                }
                else if (userID != 0 && startDate != null && endDate == null && stock == "all")
                {
                    return View(db.getUserTransactions(userID, startDate, DateTime.Now));
                }
                else if (userID == 0 && startDate != null && endDate != null && stock == "all")
                {
                    return View(db.getUserTransactions(startDate, endDate));//
                }
                else if (userID != 0 && startDate == null && endDate != null && stock == "all")
                {
                    var startD = new DateTime(1850, 1, 1);
                    return View(db.getUserTransactions(userID, startD, endDate));
                }

                //three nulls
                else if (userID == 0 && startDate == null && endDate == null && stock != "all")
                {
                    return View(db.getUserTransactions(stock));
                }
                else if (userID != 0 && startDate == null && endDate == null && stock == "all")
                {
                    return View(db.getUserTransactions(userID));
                }
                else if (userID == 0 && startDate != null && endDate == null && stock == "all")
                {
                    return View(db.getUserTransactions(startDate, DateTime.Now));
                }
                else if (userID == 0 && startDate == null && endDate != null && stock == "all")
                {
                    var startD = new DateTime(1850, 1, 1);
                    return View(db.getUserTransactions(startD, endDate));
                }
                //four nulls
                else
                {
                    return View(db.getUserTransactions());
                }
            }
            else
            {
                //no nulls
                if (userID != 0 && startDate != null && endDate != null && stock != "all")
                {
                    return View(db.getUserTransactions(userID, stock, startDate, endDate));
                }
                //one null
                else if (userID != 0 && startDate == null && endDate != null && stock != "all")
                {
                    var startD = new DateTime(1850, 1, 1);
                    return View(db.getUserTransactions(userID, stock, startD, endDate));
                }
                else if (userID != 0 && startDate != null && endDate == null && stock != "all")
                {
                    return View(db.getUserTransactions(userID, stock, startDate, DateTime.Now));
                }
                else if (userID != 0 && startDate != null && endDate != null && stock == "all")
                {
                    return View(db.getUserTransactions(userID, startDate, endDate));
                }
                //two nulls

                else if (userID != 0 && startDate == null && endDate == null && stock != "all")
                {
                    return View(db.getUserTransactions(userID, stock));
                }
                else if (userID != 0 && startDate != null && endDate == null && stock == "all")
                {
                    return View(db.getUserTransactions(userID, startDate, DateTime.Now));
                }
                else if (userID != 0 && startDate == null && endDate != null && stock == "all")
                {
                    var startD = new DateTime(1850, 1, 1);
                    return View(db.getUserTransactions(userID, startD, endDate));
                }
                //three nulls

                else
                {
                    return View(db.getUserTransactions(userID));
                }
            }
        }
    
        [Authorize]
        public ActionResult Dashboard()
        {
            User user = Session["User"] as User;
            List<Order> TodaysOrders = new List<Order>();
            List<Order> StockOrders = new List<Order>();
            List<PieChartElement> pieChartElements = new List<PieChartElement>();
            

            if (!user.Admin)
            {
                TodaysOrders = db.GetTodayOrdersForUser(user);

                StockOrders = db.GetAllStocksForUser(user);

                pieChartElements = db.GetChartDataForUser(user);

                ViewBag.TodaysOrders = TodaysOrders;
                ViewBag.StockOrders = StockOrders;



                for(int i=0; i<pieChartElements.Count; i++)
                {
                    pieChartElements[i].Stock = db.GetStock(pieChartElements[i].ID);
                }

                ViewBag.pieChartElements = pieChartElements;
 
            }
            else
            {
               return RedirectToAction("Users", "Home");
            }
            
            return View();
        }


        [Authorize]
        public ActionResult Users()
        {
            if ((Session["User"] as User).Admin == false)
            {
               return Logout();
            }
            var users = db.GetUsersList();
            return View(users);
        }
        [HttpPost]
        [Authorize]
        public ActionResult Users(string selectedMethod, string filterValue)
        {

            if ((Session["User"] as User).Admin==false)
            {
                return Logout();
            }

            List<User> users = new List<User>();

            if (filterValue == "" && selectedMethod != Resources.Resources.DefaultSearch)
            {
                ViewBag.error = Resources.Resources.EmptyFilterValue;
                return View(users);
            }

            if (selectedMethod == Resources.Resources.SearchByStatus)
            {
                if (filterValue == Resources.Resources.Blocked)
                {
                    users = db.SelectByStatues(true);
                }
                else if (filterValue == Resources.Resources.Activated)
                {
                    users = db.SelectByStatues(false);
                }

            }
            else if (selectedMethod == Resources.Resources.SearchByEmail)
            {
                User user = db.SelectByEmail(filterValue);
                if (user!=null)
                    users.Add(user);
            }
            else if (selectedMethod == Resources.Resources.SearchByUsername)
            {
                User user = db.SelectByUsername(filterValue);
                if (user != null)
                    users.Add(user);
            }
            else
            {
                users = db.GetUsersList();
            }

            
            return View(users);
        }

        [Authorize]
        public ActionResult Activate(string username)
        {
            db.ActivateUser(username);
            return RedirectToAction("Users");
        }

        [Authorize]
        public ActionResult Order()
        {
            var user = Session["User"] as User;
            if (user.Admin) { RedirectToAction("Monitor"); }
            List<Order> Valid = db.ValidToUpdate(user);
            return View(Valid);
        }
        
        [Authorize]
        [HttpPost]
        public ActionResult Order(string type, int stockID = 0, int quantity = 0,  int orderID = 0)
        {
            var user = Session["User"] as User;
            if (orderID != 0)
            {
                Order order = db.SearchUserOrders(user.Username, orderID);
                List<Order> o = new List<Order>();
                if(order != null)
                    o.Add(order);
                return View(o);
            }
            else if(type!= null && stockID != 0 && quantity!=0)
            {
                var stock = db.GetStock(stockID);
                if (!db.AddOrder(user.Username, type, stock, quantity))
                {
                    ViewBag.error = Resources.Resources.AddOrderError;
                }
                if(type == "Buy") { ViewBag.Message = Resources.Resources.You_llBeCharged + " " + stock.Price * quantity + " " + Resources.Resources.EGP; }
                if (user.Admin) { RedirectToAction("Monitor"); }
                List<Order> Valid = db.ValidToUpdate(user);
                return View(Valid);
            }
            else
            {
                return RedirectToAction("Order");
            }
        }

        [Authorize]
        public ActionResult UpdateOrder(int id)
        {
            var user = Session["User"] as User;
            if (user.Admin) { return RedirectToAction("Monitor"); }
            Order order = db.GetOrder(id);
            return View(order);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrder(int ID, int Quantity)
        {
            User user = Session["User"] as User;
            if(!db.updateOrder(user, ID, Quantity)) { ViewBag.error = Resources.Resources.ActionNotAllowed; }
            else { return RedirectToAction("Monitor"); }
            return View();
        }
    
        //[Authorize]
        //public ActionResult Order(string username, string type, string stockName, int quantity)
        //{
        //    if (!db.AddOrder(username, type, stockName, quantity))
        //    {
        //        ViewBag.error = "You don't have enough money!";
        //    }
        //    return View();
        //}



        [Authorize]
        public ActionResult Logout()
        {
            Session["User"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public ActionResult SetCulture(string culture)
        {
            CultureManager.CurrentCulture = culture;
            Session["CurrentCulture"] = culture;
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}