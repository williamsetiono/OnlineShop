using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OnlineShop.Core.Models.Users
{
    public class RegisterViewModel
    {
        [Required]
        [DisplayName(@"User name")]
        public string UserName { get; set; }
        
        [Required]
        [DisplayName(@"Password")]
        public string Password { get; set; }
        [Required]
        [DisplayName(@"Confirm password")]
        [Compare("Password"/*, ErrorMessage = "The password and confirmation password do not match."*/)]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
