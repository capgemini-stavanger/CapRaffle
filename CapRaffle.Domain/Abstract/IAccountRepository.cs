using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapRaffle.Domain.Abstract
{
    public interface IAccountRepository
    {
        bool Authenticate(string email, string password);
        bool Create(string email, string password, string name);
        bool ChangePassword(string email, string newPassword);
        bool Delete(string email);
    }
}
