using System.ComponentModel.DataAnnotations;

namespace CapRaffle.Models
{
    public class LogOnViewModel
    {
        [Required(ErrorMessage="Email requierd")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}