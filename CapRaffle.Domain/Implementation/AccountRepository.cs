using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using System.Web.Security;

namespace CapRaffle.Domain.Implementation
{
    public class AccountRepository : IAccountRepository
    {
        private CapRaffleContext context = new CapRaffleContext();

        public IQueryable<User> Users
        {
            get { return context.Users; }
        }

        public bool Authenticate(string email, string password)
        {
            email = email.ToLower();
            User user = Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                String passwd = CreatePasswordHash(password, CreateSalt(email));
                if (user.Password.Equals(passwd))
                {
                    FormsAuthentication.SetAuthCookie(email, false);
                    return true;
                }
            }
            return false;
        }

        public bool Create(string email, string password, string name)
        {
            User user = new User();
            email = email.ToLower();
            String passwd = CreatePasswordHash(password, CreateSalt(email));
            user.Email = email;
            user.Password = passwd;
            user.Name = name;
            context.AddToUsers(user);
            context.SaveChanges();
            return true;
        }

        public bool ChangePassword(string email, string newPassword)
        {
            email = email.ToLower();
            User user = Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                String passwd = CreatePasswordHash(newPassword, CreateSalt(email));
                user.Password = passwd;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        private static string CreateSalt(string email)
        {
            String start = email.Substring(0, 3);
            String end = email.Substring(2, 2);
            return String.Concat(start, end);
        }

        private static string CreatePasswordHash(string password, string salt)
        {
            string saltAndPassword = String.Concat(password, salt);
            string hashedPassword =
             FormsAuthentication.HashPasswordForStoringInConfigFile(
             saltAndPassword, "sha1");

            return hashedPassword;
        }
    }
}
