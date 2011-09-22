using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

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
            [Required(ErrorMessage = "Please enter your capgemini.com email address")]
            [Remote("EmailExists", "Account", ErrorMessage = "Email already exists")]
            [RegularExpression(".+\\@capgemini.com", ErrorMessage = "Please enter a valid capgemini.com email address")]
            public string Email { get; set; }
            
            [Required(ErrorMessage = "Please enter your name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Password requierd")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
