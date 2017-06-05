using System;
using System.Linq;
using OnlineShop.Core.Models.Users;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.Core.Services.Users
{
    public class UserService : BaseService<Account>,IUserService
    {
        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Account FindByName(string userName)
        {
            return Repository.GetAll(a => a.Username.Equals(userName)).FirstOrDefault();
        }

        public Account FindById(int id)
        {
            return Repository.Get(id);
        }

        public IQueryable<Account> GetAll()
        {
            return Repository.GetAll();
        }

        public void Insert(UserViewModel model)
        {
            var user = FindByName(model.UserName);
            if (user != null) throw new Exception(string.Format("User name {0} is exist",model.UserName));
            user = MapToUser(model, null, true);
            Repository.Insert(user);
            UnitOfWork.Save();
        }
        public void Insert(RegisterViewModel model)
        {
            var user = FindByName(model.UserName);
            if (user != null) throw new Exception(string.Format("User name {0} is exist", model.UserName));
            model.Password = Utils.UtilEncrypt.GetMd5Hash(model.Password);
            user = new Account()
            {
                Username = model.UserName,
                Password = model.Password,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                Status = true,
                Role = DefaultRole()
            };
            Repository.Insert(user);
            UnitOfWork.Save();
        }
        public void Update(UserViewModel model)
        {
            var user = FindById(model.Id);
            if (user == null) throw new Exception(string.Format("User name {0} is not exist", model.UserName));
            user = MapToUser(model,user,!string.IsNullOrWhiteSpace(model.Password));
            Repository.Update(user);
            UnitOfWork.Save();
        }

        public void Update(UserInfoViewModel model)
        {
            var user = Repository.GetAll(a => a.Username.Equals(model.UserName)).FirstOrDefault();
            if(user == null) throw new Exception("User is not found");
            user.Phone = model.Phone;
            user.Address = model.Address;
            user.Email = model.Email;
            user.FullName=model.FullName;
            Repository.Update(user);
            UnitOfWork.Save();
        }

        public void Delete(int id)
        {
            var user = FindById(id);
            if (user == null) throw new Exception(string.Format("User id is not exist"));
            
            Repository.Delete(user);
            UnitOfWork.Save();
        }

        private Account MapToUser(UserViewModel model,Account user = null, bool changePwd = false)
        {
            if(user == null) user = new Account();
            if (model == null) return null;
            Utils.UtilEncrypt.GetMd5Hash(model.Password);

            user.Status = model.Status;
            user.Address = model.Address;
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.Password = changePwd ? model.Password : user.Password;
            user.RoleId = model.RoleId;
            user.Username = model.UserName;
            user.Phone = model.Phone;
            return user;
        }

        private Role DefaultRole()
        {
            var roles = UnitOfWork.Repository<Role>().GetAll();
            return roles.Any(a => a.Name.Equals("User"))
                ? roles.FirstOrDefault(a => a.Name.Equals("User"))
                : roles.FirstOrDefault(a => a.Name != "Admin");
        }
    }
}
