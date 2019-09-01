﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UTW_Project.Models;

namespace UTW_Project.Classes
{
    public class DBManager
    {

        private UTWProjectEntities Db = new UTWProjectEntities();


        public void Add(User user)
        {
            Db.Users.Add(user);
            Db.SaveChanges();
        }

        public void AddURL(string urlString, Guid guid, User user)
        {
            URL url = new URL();
            url.Date = DateTime.Now;
            url.GUID = guid;
            url.URL1 = urlString;

            url = Db.URLs.Add(url);
            user.URL_ID = url.ID;
            Db.SaveChanges();
        }
        public User GetUser(string username)
        {
            var query = from u in Db.Users where u.Username == username select u;
            return query.FirstOrDefault();
        }

        public User GetUrlUser(Guid guid)
        {
            var query = from u in Db.Users where u.URL.GUID == guid select u;
            User user = query.FirstOrDefault();
            if (DateTime.Now > user.URL.Date.AddDays(1))
                user.URL.Expired = true;
            Db.SaveChanges();
            return user;
        }
        public bool UserExists(string username)
        {
            if (GetUser(username) == null)
                return false;
            return true;
        }

        public void UpdateTrials(string username)
        {
            User user = GetUser(username);
            if (user != null)
            {
                user.LoginTrials += 1;
                Db.SaveChanges();
            }

        }

        public void ActivateUser(string username)
        {
            User user = GetUser(username);
            user.Blocked = false;
            user.LoginTrials = 2;
            Db.SaveChanges();
        }

        public bool HasAccount(string email)
        {
            var query = from u in Db.Users where u.Email == email select u;
            if (query.FirstOrDefault() == null)
                return false;
            return true;
        }
        public bool EmailConfirm(User user)
        {

            if (user != null)
            {
                user.EmailConfirmed = true;
                user.URL.Expired = true;
                Db.SaveChanges();
                return true;
            }
            return false;
        }

        public void ResetPassword(User user, string newPassword)
        {
            user.URL.Expired = true;
            user.Password = user.MD5Hash(newPassword);
            Db.SaveChanges();
        }

        public List<Question> GetQuestions()
        {
            return Db.Questions.ToList();
        }

        public List<User> GetUsersList()
        {
            return Db.Users.ToList();
        }

        public List<User> SelectByStatues(bool value)
        {
            var query = from u in Db.Users where u.Blocked == value select u;
            return query.ToList();
        }

        public User SelectByEmail(string value)
        {
            var query = from u in Db.Users where u.Email == value select u;
            return query.FirstOrDefault();

        }

        public User SelectByUsername(string value)
        {
            return GetUser(value);
        }

        public Stock GetStock(string name)
        {
            var query = from s in Db.Stocks where s.CompanyEN == name select s;
            return query.FirstOrDefault();
        }

        public Order Search(int orderID)
        {
            var query = from o in Db.Orders where o.ID == orderID select o;
            return query.FirstOrDefault();
        }

        public void updateOrder(int id, int quantity)
        {
            Order order = Search(id);
            order.Quantity = quantity;
            Db.SaveChanges();

        }

        public List<Order> ValidToUpdate()
        {
            var date = DateTime.Now.Date;
            var query = from o in Db.Orders where o.Date == date select o;
            return query.ToList();
        }

        public bool AddOrder(string username, string type, string stockName, int quantity)
        {
            User user = GetUser(username);
            Stock stock = GetStock(stockName);

            int UID = user.ID;
            int SID = stock.ID;

            Order order = new Order();

            order.U_ID = UID;
            order.S_ID = SID;
            order.Quantity = quantity;
            order.Date = DateTime.Now.Date;
            order.StateEN = "A";
            order.TypeEN = type;

            if (type == "Buy")
            {
                if (user.Wallet < (quantity * stock.Price))
                {
                    return false;
                }
            }
            Db.Orders.Add(order);
            Db.SaveChanges();
            return true;
        }


        //User
        // Monitor Transactions: Gets Orders by specific User 
        public List<Order> getUserTransactions(int id)
        {
            var query = from u in Db.Orders where u.U_ID == id select u;
            return query.ToList();
        }

        //User, Admin
        // Monitor Transactions: Gets Orders by specific User and between two dates
        public List<Order> getUserTransactions(int id, DateTime startDate, DateTime endDate)
        {
            if (startDate <= endDate)
            {
                var query = from u in Db.Orders where u.U_ID == id && (u.Date >= startDate && u.Date <= endDate) select u;
                return query.ToList();
            }
            else
            {
                var query = from u in Db.Orders where u.U_ID == id select u;
                return query.ToList();
            }
        }

        //Gets stock id for stock with specific name
        public int getStockID(string name)
        {
            var query = from u in Db.Stocks where u.CompanyEN == name || u.CompanyAR == name select u.ID;
            return query.FirstOrDefault();
        }
        //User, Admin
        // Monitor Transactions: Gets Orders by specific User and for specific stock
        public List<Order> getUserTransactions(int id, string stock)
        {
            var query = from u in Db.Orders where u.U_ID == id && u.S_ID == getStockID(stock) select u;
            return query.ToList();
        }

        //User, Admin 
        // Monitor Transactions: Gets Orders by specific User and for specific stock
        public List<Order> getUserTransactions(int id, string stock, DateTime startDate, DateTime endDate)
        {
            if (startDate <= endDate)
            {
                var query = from u in Db.Orders where u.U_ID == id && u.S_ID == getStockID(stock) && (u.Date >= startDate && u.Date <= endDate) select u;
                return query.ToList();
            }
            else return getUserTransactions(id, stock);
        }

        //Admin
        // Monitor Transactions: Gets Orders by all users
        public List<Order> getUserTransactions()
        {
            var query = from u in Db.Orders select u;
            return query.ToList();
        }

        //Admin
        // Monitor Transactions: Gets Orders by all Users and between two dates
        public List<Order> getUserTransactions(DateTime startDate, DateTime endDate)
        {
            if (startDate <= endDate)
            {
                var query = from u in Db.Orders where (u.Date >= startDate && u.Date <= endDate) select u;
                return query.ToList();
            }
            else
            {
                var query = from u in Db.Orders select u;
                return query.ToList();
            }
        }

        //Admin
        // Monitor Transactions: Gets Orders for specific stock
        public List<Order> getUserTransactions(string stock)
        {
            var query = from u in Db.Orders where u.S_ID == getStockID(stock) select u;
            return query.ToList();
        }

        //Admin 
        // Monitor Transactions: Gets Orders for specific stock
        public List<Order> getUserTransactions(string stock, DateTime startDate, DateTime endDate)
        {
            if (startDate <= endDate)
            {
                var query = from u in Db.Orders where u.S_ID == getStockID(stock) && (u.Date >= startDate && u.Date <= endDate) select u;
                return query.ToList();
            }
            else return getUserTransactions(stock);
        }

    }
}