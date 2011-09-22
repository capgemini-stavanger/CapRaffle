using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Abstract
{
    public interface IAccountRepository
    {
        IQueryable<User> Users { get; }

        bool Authenticate(string email, string password);
        bool Create(string email, string password, string name);
        bool ChangePassword(string email, string newPassword);
        void SignOut();
        void ForgotPassword(string email);
        User GetUserByEmail(string email);
    }
}
