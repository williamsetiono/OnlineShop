using System.Linq;
using OnlineShop.Core.Models.Users;
using OnlineShop.Models.Entities;

namespace OnlineShop.Core.Services.Users
{ 
    public interface IUserService : IBaseService<Account>
    {
        Account FindByName(string userName);
        Account FindById(int id);
        IQueryable<Account> GetAll();
        void Insert(UserViewModel user);
        void Insert(RegisterViewModel model);
        void Update(UserViewModel user);
        void Update(UserInfoViewModel user);
        void Delete(int id);
    }
}
