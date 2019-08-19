using System;
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

        public User GetUser(string username)
        {
            var query = from u in Db.Users where u.Username == username select u;
            return query.FirstOrDefault();
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

        public bool HasAccount(string email)
        {
            var query = from u in Db.Users where u.Email == email select u;
            if (query.FirstOrDefault() == null)
                return false;
            return true;
        }
        public bool EmailConfirm(string username)
        {
            User user = GetUser(username);
            if (user != null)
            {
                user.EmailConfirmed = true;
                Db.SaveChanges();
                return true;
            }
            return false;
        }

        public void ResetPassword(string username, string newPassword, string answer)
        {
            User user = GetUser(username);
            if (user.MD5Hash(answer) == user.Answer)
            {
                user.Password = user.MD5Hash(newPassword);
                Db.SaveChanges();
            }
        }

        public List<Question> GetQuestions()
        {
            return Db.Questions.ToList();
        }
    }
}