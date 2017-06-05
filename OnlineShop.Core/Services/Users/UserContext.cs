using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlineShop.Core.Utils;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.Core.Services.Users
{
    public class UserContext : BaseService<Account>,IUserContext
    {
        public UserContext(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Account Login(string username, string password)
        {
            password = UtilEncrypt.GetMd5Hash(password);
            return Repository.GetAll(a => a.Username.Equals(username) && a.Password.Equals(password)).FirstOrDefault();
        }

        public Account GetUser(string username)
        {
            throw new NotImplementedException();
        }
    }
}
