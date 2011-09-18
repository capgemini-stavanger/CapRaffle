using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapRaffle.Domain.Abstract
{
    public interface IAccountRepository
    {
        bool Authenticate(string Email, string password);
        bool Create(string Email, string password);
        bool ChangePassword(string Email, string password, string newPassword);
        bool Delete(string Email);
    }
}
