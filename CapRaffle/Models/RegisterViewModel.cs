using System.ComponentModel.DataAnnotations;

namespace CapRaffle.Models
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Please enter your capgemini.com email address")]
        [RegularExpression(".+\\@capgemini.com",
        ErrorMessage = "Please enter a valid capgemini.com email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password requierd")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password repeated requierd")]
        [DataType(DataType.Password)]
        public string PasswordAgain { get; set; }

    }
}