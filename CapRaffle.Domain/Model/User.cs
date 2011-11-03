using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace CapRaffle.Domain.Model
{
    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {

        [Required(ErrorMessage = "Password requierd")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match."),]
        public string PasswordAgain { get; set; }

        public class UserMetaData
        {
            [Remote("EmailExists", "Account", ErrorMessage = "Email already exists")]
            [RequiredAndValidEmail(ErrorMessage = "Please enter your domain email address")]
            public string Email { get; set; }
            
            [Required(ErrorMessage = "Please enter your name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Password requierd")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
