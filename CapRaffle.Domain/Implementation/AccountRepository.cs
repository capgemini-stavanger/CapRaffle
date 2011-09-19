using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Implementation
{
    public class AccountRepository : IAccountRepository
    {
        public bool Authenticate(string email, string password)
        {
            return false;
        }

        public bool Create(string email, string password, string name)
        {
            CapRaffleContext context = new CapRaffleContext();
            User user = new User();
            user.Email = email;
            user.Password = password;
            context.AddToUsers(user);
            context.SaveChanges();
            return true;
        }

        public bool ChangePassword(string email, string newPassword)
        {
            return false;
        }

        public bool Delete(string email)
        {
            return false;
        }
    }
}
