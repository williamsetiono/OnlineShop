using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlineShop.Models.Entities;

namespace OnlineShop.Core.Services.Users
{
    public interface IUserContext: IBaseService<Account>
    {
        Account Login(string username, string password);
        Account GetUser(string username);
    }
}
