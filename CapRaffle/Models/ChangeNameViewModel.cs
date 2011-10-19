using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CapRaffle.Models
{
    public class ChangeNameViewModel
    {
        public string Email { get; set; }

        [Display(Name = "Change your name")]
        [Required(ErrorMessage = "Please enter your name")]
        public string NewName { get; set; }
    }
}