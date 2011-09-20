﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CapRaffle.Models
{
    public class ChangePasswordViewModel
    {

        [Required(ErrorMessage = "Please enter your capgemini.com email address")]
        [RegularExpression(".+\\@capgemini.com",
        ErrorMessage = "Please enter a valid capgemini.com email address")]
        [HiddenInput(DisplayValue = false)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password requierd")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password requierd")]
        [DataType(DataType.Password)]
        [Display(Name="Repeat password")]
        public string PasswordAgain { get; set; }
    }
}