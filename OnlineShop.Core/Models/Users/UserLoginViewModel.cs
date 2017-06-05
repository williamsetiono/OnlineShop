using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Identity;

namespace OnlineShop.Core.Models.Users
{
    public class UserLoginViewModel : IUser
    {
        public UserLoginViewModel()
        {
            //Id = Guid.NewGuid().ToString();
        }
        public string Id { get; private set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}
