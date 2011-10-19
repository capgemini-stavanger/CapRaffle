using System;
using System.Linq;
using System.Web.Security;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;

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
            User user = GetUserByEmail(email);
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
            if (string.IsNullOrEmpty(password)) password = GeneratePassword();

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
            User user = GetUserByEmail(email);
            if (user != null)
            {
                String passwd = CreatePasswordHash(newPassword, CreateSalt(email));
                user.Password = passwd;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ChangeName(string email, string newName)
        {
            User user = GetUserByEmail(email);
            if (user != null)
            {
                user.Name = newName;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public User GetUserByEmail(string email)
        {
            email = email.ToLower();
            return Users.FirstOrDefault(u => u.Email == email);
        }

        public void ForgotPassword(string email)
        {
            IEmailSender emailSender = new EmailSender();

            string newPassword = GeneratePassword();
            ChangePassword(email, newPassword);
            emailSender.ForgotPassword(email, newPassword);
        }

        private string CreateSalt(string email)
        {
            String start = email.Substring(0, 3);
            String end = email.Substring(2, 2);
            return String.Concat(start, end);
        }

        private string CreatePasswordHash(string password, string salt)
        {
            string saltAndPassword = String.Concat(password, salt);
            string hashedPassword =
             FormsAuthentication.HashPasswordForStoringInConfigFile(
             saltAndPassword, "sha1");

            return hashedPassword;
        }

        private string GeneratePassword()
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[8];
            Random rd = new Random();

            for (int i = 0; i < 8; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
    }
}
