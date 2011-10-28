using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapRaffle.Domain.Model;

namespace CapRaffle.Domain.Abstract
{
    public interface IEmailSender
    {
        bool ForgotPassword(string email, string newPassword);
        bool NotifyLooser(UserEvent looser);
        bool NotifyWinner(Winner winner);
    }
}
