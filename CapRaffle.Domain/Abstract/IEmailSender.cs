using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapRaffle.Domain.Abstract
{
    public interface IEmailSender
    {
        void ForgotPassword(string email, string newPassword);
    }
}
