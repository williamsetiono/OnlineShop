using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineShop.Core.Models.Users
{
    public class UserViewModel{
        
        public int Id { get; set ; }
        [Required]
        [RegularExpression("[\\w]+",ErrorMessage = "{0} matches an alphanumeric character, including '_'")]
        [Display(Name = @"User name")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [DisplayName(@"Confirm password")]
        [Compare("Password"/*, ErrorMessage = "The password and confirmation password do not match."*/) ]
        public string PasswordConfirm { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public string Role { get; set; }
        [Required]
        [DisplayName(@"Role")]
        public int RoleId { get; set; }
        public Dictionary<int,string> Roles { get; set; }
    }
}
