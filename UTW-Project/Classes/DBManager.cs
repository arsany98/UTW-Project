using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccessLayer;

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
            if (user!=null)
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

    }
}